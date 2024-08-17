using acl_openstack_identity.Data;
using acl_openstack_identity.features;
using acl_openstack_identity.Helpers;
using acl_openstack_identity.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Text;
using System.Text.Json;

namespace acl_openstack_identity.Controllers
{
    [Route("api/register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly OpenstackContext _context;
        private readonly IConfiguration _configuration;
        private organizations _organizations;
        private readonly HttpClient _httpClient;

        public RegisterController (OpenstackContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
            _organizations = new organizations(configuration, context, httpClient);
        }

        /// <summary>
        /// Handles user registration requests, which involve creating a user in Keycloak, 
        /// OpenStack, and associating them with an organization.
        /// </summary>
        /// <param name="reg">The registration request object containing user details.</param>
        /// <returns>An IActionResult indicating the outcome of the registration process.</returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] registerReqOb reg)
        {
            // Validate that the required fields are provided in the registration request
            if (string.IsNullOrEmpty(reg.name))
                return new JsonResult(new { result = "no_name" });
            if (string.IsNullOrEmpty(reg.lastname))
                return new JsonResult(new { result = "no_lastname" });
            if (string.IsNullOrEmpty(reg.email))
                return new JsonResult(new { result = "no_email" });
            if (string.IsNullOrEmpty(reg.password) || string.IsNullOrEmpty(reg.repeatPassword))
                return new JsonResult(new { result = "no_password" });
            if (string.IsNullOrEmpty(reg.organizationName))
                return new JsonResult(new { result = "no_organization_name" });

            // Check if the password and repeat password fields match
            if (reg.password != reg.repeatPassword)
                return new JsonResult(new { result = "password_not_match" });

            try
            {
                // Check if a user with the same email already exists in the database
                var checkUser = _context.Users.FirstOrDefault(u => u.Email == reg.email.Trim());
                if (checkUser != null)
                    return new JsonResult(new { result = "exists" });

                // Generate a random password for OpenStack
                var openstackPassword = generators.generatePassword(20);

                // Create a new organization in the system
                var organizationResult = await _organizations.CreateOrganization(new AddOrganizationOb
                {
                    name = reg.organizationName,
                });

                // If the organization creation failed, return an error
                if (organizationResult.result != "organization_created")
                    return new JsonResult(new { result = "error" });

                // Retrieve the newly created organization from the database
                var organization = _context.Projects.FirstOrDefault(o => o.Id == organizationResult.organizationId);
                if (organization == null)
                    return new JsonResult(new { result = "error" });

                // Function to get a Keycloak access token
                async Task<string> getKeycloakAccessToken()
                {
                    var tokenEndpoint = $"{_configuration["KeyCloak:Url"]}/realms/master/protocol/openid-connect/token";

                    var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "admin-cli" },
                { "username",  _configuration["KeyCloak:Username"]},
                { "password", _configuration["KeyCloak:Password"] }
            };

                    HttpClient client = new HttpClient();

                    // Send a POST request to Keycloak to obtain the access token
                    var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));
                    if (!response.IsSuccessStatusCode)
                        return "error";

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenData = JObject.Parse(responseContent);

                    return tokenData["access_token"].ToString();
                }

                // Get Keycloak access token for user creation
                var token = await getKeycloakAccessToken();
                if (token == "error")
                    return new JsonResult(new { result = "error" });

                // Function to create a new user in Keycloak
                async Task<string> keycloakCreateUser()
                {
                    var createUserEndpoint = $"{_configuration["KeyCloak:Url"]}/admin/realms/acl_openstack/users";

                    var userKeyCloak = new
                    {
                        username = reg.email,
                        email = reg.email,
                        firstName = reg.name,
                        lastName = reg.lastname,
                        enabled = true,
                        credentials = new[]
                        {
                    new
                    {
                        type = "password",
                        value = reg.password,
                        temporary = false
                    }
                }
                    };

                    HttpClient client = new HttpClient();

                    var jsonContent = new StringContent(JsonSerializer.Serialize(userKeyCloak), Encoding.UTF8, "application/json");
                    if (_httpClient.DefaultRequestHeaders.Authorization != null)
                        _httpClient.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a POST request to Keycloak to create the user
                    var response = await client.PostAsync(createUserEndpoint, jsonContent);
                    if (!response.IsSuccessStatusCode)
                        return "error";
                    else
                        return "user_created";
                }

                // Create the user in Keycloak
                var createKeycloakUserStatus = await keycloakCreateUser();
                if (createKeycloakUserStatus == "error")
                    return new JsonResult(new { result = "error" });

                // Authenticate with OpenStack using the admin user's credentials
                OpenstackClient _client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    _configuration["OpenStack:Username"],
                    _configuration["OpenStack:Password"],
                    _configuration["OpenStack:Domain"],
                    _configuration["OpenStack:Project"]
                );

                var openstackToken = await _client.Authenticate();

                // Create the user in OpenStack
                User userOpenstack = new User(openstackToken.identityUrl, _httpClient);

                var addUserOpenstackResult = await userOpenstack.CreateUser(openstackToken.token, new ReourcesRequestsObjects.UserRequest.UserRequestAdd
                {
                    defaultProjectId = organization.OpenstackProjectId,
                    domain_id = _configuration["OpenStack:Domain"],
                    enabled = true,
                    name = reg.email.ToString(),
                    password = openstackPassword
                });

                if (addUserOpenstackResult.name == "error")
                    return new JsonResult(new { result = "error" });

                // Add the new user to the database
                var newUser = _context.Users.Add(new Models.User
                {
                    Name = reg.name.Trim(),
                    Lastname = reg.lastname.Trim(),
                    Email = reg.email.Trim(),
                    Phone = reg.phone.Trim(),
                    Isfirstlogin = false,
                    OpenstackId = addUserOpenstackResult.Id,
                    Openstackpassword = openstackPassword
                });

                await _context.SaveChangesAsync();

                // Add the user to the organization as an owner
                var addToOrgResult = await _organizations.AddOwnerToOrganisation((int)organizationResult.organizationId, (int)newUser.Entity.Id);
                if (addToOrgResult != "user_added")
                    return new JsonResult(new { result = "error" });

                // Return success result
                return new JsonResult(new { result = "account_registered" });
            }
            catch (NpgsqlException ex)
            {
                // Log the exception if an error occurs
                Logger.SendException("Openstack_Panel", "RegisterController", "Register", ex);
                return new JsonResult(new { result = "error" });
            }
        }
    }

    public class registerReqOb
    {
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? password { get; set; }
        public string? repeatPassword { get; set; }
        public string? organizationName { get; set; }
    }
}
