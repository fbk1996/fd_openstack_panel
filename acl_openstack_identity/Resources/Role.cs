using acl_openstack_identity.Helpers;

namespace acl_openstack_identity.Resources
{
    public class Role
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use for HTTP requests.</param>
        /// <param name="apiUrl">The base URL of the OpenStack API.</param>
        public Role(HttpClient httpClient, string apiUrl)
        {
            _httpClient = httpClient;
            _apiUrl = apiUrl;
        }

        /// <summary>
        /// Assigns a role to a group within a project in OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public async Task<string> AssignRoleToGroup(string token, string projectId, string groupId, string roleId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(roleId))
                return "error";

            try
            {
                // Add the authentication token to the request headers
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);

                // Send a PUT request to assign the role to the group within the project
                var response = await _httpClient.PutAsync($"{_apiUrl}/v3/projects/{projectId}/groups/{groupId}/roles/{roleId}", null);

                // Check if the response indicates success
                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "role_added";
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur
                Logger.SendNormalException("acl_openstack", "Role", "AssignRoleToGroup", ex);
                return "error";
            }
        }
    }
}
