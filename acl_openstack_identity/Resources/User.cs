namespace acl_openstack_identity.Resources
{
    using acl_openstack_identity.Helpers;
    using Newtonsoft.Json.Linq;
    using System.Dynamic;
    using System.Text;
    using static acl_openstack_identity.ReourcesRequestsObjects.UserRequest;
    using static acl_openstack_identity.ResourceTypes.UserTypes;

    /// <summary>
    /// Represents a user management class for interacting with OpenStack API.
    /// </summary>
    public class User
    {
        private readonly HttpClient _httpClient; // HttpClient instance to make HTTP requests
        private readonly string _apiPath; // Base URL of the API

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="apiPath">The base path of the API.</param>
        public User(string apiPath, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiPath = apiPath;
        }

        /// <summary>
        /// Creates a new user in OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="requestOb">The user creation request object.</param>
        /// <returns>An <see cref="OpenstackUser"/> object representing the created user.</returns>
        public async Task<OpenstackUser> CreateUser(string token, UserRequestAdd requestOb)
        {
            try
            {
                // Create a dynamic object to hold the user properties
                dynamic userOb = new ExpandoObject();

                // Conditionally add properties to the user object based on the request object
                if (!string.IsNullOrEmpty(requestOb.defaultProjectId))
                    userOb.default_project_id = requestOb.defaultProjectId;
                if (!string.IsNullOrEmpty(requestOb.domain_id))
                    userOb.domain_id = requestOb.domain_id;
                if (!string.IsNullOrEmpty(requestOb.name))
                    userOb.name = requestOb.name;
                if (!string.IsNullOrEmpty(requestOb.password))
                    userOb.password = requestOb.password;

                // Set the 'enabled' property, defaulting to true if not provided
                userOb.enabled = (requestOb.enabled != null) ? requestOb.enabled : true;

                // Create the payload for the request
                var addUserPayload = new
                {
                    user = userOb
                };

                // Serialize the payload to JSON
                var content = new StringContent(JObject.FromObject(addUserPayload).ToString(), Encoding.UTF8, "application/json");

                // Add the authorization token to the request headers
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);

                // Make the POST request to create the user
                var response = await _httpClient.PostAsync($"{_apiPath}/v3/users", content);

                // Handle the response
                if (!response.IsSuccessStatusCode)
                    return new OpenstackUser { name = "error" };

                // Parse the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                var userResponse = JObject.Parse(responseContent)["user"];
                var user = userResponse.ToObject<OpenstackUser>();

                // Return the created user or an error object if parsing failed
                return user ?? new OpenstackUser { name = "error" };
            }
            catch (Exception ex)
            {
                // Log the exception and return an error object
                Logger.SendNormalException("acl_openstack", "User", "CreateUser", ex);
                return new OpenstackUser { name = "error" };
            }
        }

        /// <summary>
        /// Edits an existing user in OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="requestOb">The user edit request object.</param>
        /// <returns>A string indicating the result of the edit operation.</returns>
        public async Task<string> EditUser(string token, UserRequestEdit requestOb)
        {
            // Return an error if the user ID is null or empty
            if (string.IsNullOrEmpty(requestOb.id))
                return "error";

            try
            {
                // Create a dynamic object to hold the user properties
                dynamic userOb = new ExpandoObject();

                // Conditionally add properties to the user object based on the request object
                if (!string.IsNullOrEmpty(requestOb.name))
                    userOb.name = requestOb.name;
                userOb.enabled = (requestOb.enabled != null) ? requestOb.enabled : true;

                // Create the payload for the request
                var EditUserPayload = new
                {
                    user = userOb
                };

                // Serialize the payload to JSON
                var content = new StringContent(JObject.FromObject(EditUserPayload).ToString(), Encoding.UTF8, "application/json");

                // Add the authorization token to the request headers
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);

                // Make the PATCH request to edit the user
                var response = await _httpClient.PatchAsync($"{_apiPath}/v3/users/{requestOb.id}", content);

                // Handle the response
                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "user_edited";
            }
            catch (Exception ex)
            {
                // Log the exception and return an error message
                Logger.SendNormalException("acl_openstack", "User", "EditUser", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes an existing user in OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A string indicating the result of the delete operation.</returns>
        public async Task<string> DeleteUser(string token, string id)
        {
            // Return an error if the user ID is null or empty
            if (string.IsNullOrEmpty(id))
                return "error";

            try
            {
                // Add the authorization token to the request headers
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);

                // Make the DELETE request to delete the user
                var response = await _httpClient.DeleteAsync($"{_apiPath}/v3/users/{id}");

                // Handle the response
                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "user_deleted";
            }
            catch (Exception ex)
            {
                // Log the exception and return an error message
                Logger.SendNormalException("acl_openstack", "User", "DeleteUser", ex);
                return "error";
            }
        }
    }
}
