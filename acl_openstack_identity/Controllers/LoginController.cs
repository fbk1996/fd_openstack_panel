using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace acl_openstack_identity.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly OpenstackContext _context;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;
        private cookieToken _token;

        public LoginController (OpenstackContext context, IConfiguration configuration, HttpClient client)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = client;
            _token = new cookieToken(client, configuration);
        }

        /// <summary>
        /// Handles user login requests, validates the Keycloak token, and issues a JWT token if the user is authenticated.
        /// </summary>
        /// <param name="login">The login request object containing user credentials or a token.</param>
        /// <returns>An IActionResult indicating the outcome of the login process.</returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] loginOb login)
        {
            // Validate that either the token or the email is provided in the login request
            if (string.IsNullOrEmpty(login.token) && string.IsNullOrEmpty(login.email))
                return StatusCode(400, new { result = "no_data" });

            try
            {
                // Function to validate the Keycloak token
                async Task<bool> ValidateKeyCloakToken(string token)
                {
                    // Construct the URL for the Keycloak token introspection endpoint
                    var url = $"{_configuration["KeyCloak:Url"]}/realms/{_configuration["KeyCloak:Realm"]}/protocol/openid-connect/token/introspect";

                    // Prepare the request data for the token introspection request
                    var requestData = new Dictionary<string, string>
            {
                { "token", token },
                { "client_id", _configuration["KeyCloak:ClientId"] },
                { "client_secret", _configuration["KeyCloak:Secret"] }
            };

                    // Send the token introspection request to Keycloak
                    var requestContent = new FormUrlEncodedContent(requestData);
                    var response = await _httpClient.PostAsync(url, requestContent);

                    // If the response is successful, check if the token is active
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                        return content != null && content.TryGetValue("active", out var isActive) && (bool)isActive;
                    }

                    // Return false if the token is invalid or if the request fails
                    return false;
                }

                // Validate the provided Keycloak token
                var isKeycloakValidate = await ValidateKeyCloakToken(login.token);

                // If the token validation fails, return a 401 Unauthorized status
                if (!isKeycloakValidate)
                    return StatusCode(401, new { result = "no_auth" });

                // Retrieve the user from the database using the provided email
                var user = _context.Users.FirstOrDefault(u => u.Email == login.email);

                // If the user is not found, return a 500 Internal Server Error status
                if (user == null)
                    return StatusCode(500, new { result = "error" });

                // Get the JWT token for the authenticated user
                var jwtToken = await _token.GetToken((int)user.Id);

                // If the JWT token generation fails, return a 401 Unauthorized status
                if (jwtToken == "error")
                    return StatusCode(401, new { result = "no_auth" });

                // Set cookie options for the JWT token
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddHours(2),
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                // Add the JWT token as a cookie in the response
                Response.Cookies.Append("openstack_token", jwtToken, cookieOptions);

                // Return an OK status with a success message
                return Ok(new { result = "logged_in" });
            }
            catch (NpgsqlException ex)
            {
                // Log the exception if a database error occurs
                Logger.SendException("Openstack_Panel", "LoginController", "Login", ex);
                return StatusCode(500, new { result = "error" });
            }
        }
    }

    public class loginOb
    {
        public string? token { get; set; }
        public string? email { get; set; }
    }
}
