namespace acl_openstack_identity.Resources
{
    using static acl_openstack_identity.ResourceTypes.GroupTypes;
    using static acl_openstack_identity.ReourcesRequestsObjects.GroupRequest;
    using acl_openstack_identity.Helpers;
    using System.Dynamic;
    using Newtonsoft.Json.Linq;
    using System.Text;

    /// <summary>
    /// This class provides methods to manage OpenStack groups, including adding, editing,
    /// deleting groups, and adding or removing users from groups.
    /// </summary>
    public class Group
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        /// <summary>
        /// Initializes a new instance of the Group class.
        /// </summary>
        /// <param name="apiUrl">The base URL of the OpenStack API.</param>
        public Group(string apiUrl, HttpClient httpClient)
        {
            _apiUrl = apiUrl;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Adds a new group to OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="requestOb">The request object containing group details.</param>
        /// <returns>The created OpenStack group.</returns>
        public async Task<OpenstackGroup> AddGroup(string token, AddGroupRequest requestOb)
        {
            if (string.IsNullOrEmpty(requestOb.name))
                return new OpenstackGroup { name = "error" };

            try
            {
                dynamic reqBody = new ExpandoObject();

                if (!string.IsNullOrEmpty(requestOb.name))
                    reqBody.name = requestOb.name;
                if (!string.IsNullOrEmpty(requestOb.description))
                    reqBody.description = requestOb.description;
                if (!string.IsNullOrEmpty(requestOb.domainId))
                    reqBody.domain_id = requestOb.domainId;

                var groupPayload = new
                {
                    group = reqBody
                };

                var content = new StringContent(JObject.FromObject(groupPayload).ToString(), Encoding.UTF8, "application/json");
                if (_httpClient.DefaultRequestHeaders.Contains("X-Auth-Token"))
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                var response = await _httpClient.PostAsync($"{_apiUrl}/v3/groups", content);

                if (!response.IsSuccessStatusCode)
                    return new OpenstackGroup { name = $"error {response.StatusCode}" };

                var responseContent = await response.Content.ReadAsStringAsync();
                var groupResponse = JObject.Parse(responseContent)["group"];
                var group = groupResponse.ToObject<OpenstackGroup>();

                

                return group ?? new OpenstackGroup { name = "error" };
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("acl_openstack", "Group", "AddGroup", ex);
                return new OpenstackGroup { name = "error" };
            }
        }

        /// <summary>
        /// Edits an existing group in OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="requestOb">The request object containing updated group details.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public async Task<string> EditGroup(string token, EditGroupRequest requestOb)
        {
            if (string.IsNullOrEmpty(requestOb.name) && string.IsNullOrEmpty(requestOb.description))
                return "nothing_to_edit";
            if (string.IsNullOrEmpty(requestOb.id))
                return "error";

            try
            {
                dynamic reqBody = new ExpandoObject();

                if (!string.IsNullOrEmpty(requestOb.name))
                    reqBody.name = requestOb.name;
                if (!string.IsNullOrEmpty(requestOb.description))
                    reqBody.description = requestOb.description;

                var groupPayload = new
                {
                    group = reqBody
                };

                var content = new StringContent(JObject.FromObject(groupPayload).ToString(), Encoding.UTF8, "application/json");
                if (_httpClient.DefaultRequestHeaders.Contains("X-Auth-Token"))
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                var response = await _httpClient.PatchAsync($"{_apiUrl}/v3/groups/{requestOb.id}", content);

                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "group_edited";
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("acl_openstack", "Group", "EditGroup", ex);
                return "error";
            }
        }

        /// <summary>
        /// Adds a user to an existing group in OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public async Task<string> AddUserToGroup(string token, string groupId, string userId)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(userId))
                return "error";

            try
            {
                if (_httpClient.DefaultRequestHeaders.Contains("X-Auth-Token"))
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                var response = await _httpClient.PutAsync($"{_apiUrl}/v3/groups/{groupId}/users/{userId}", null);

                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "user_added_to_group";
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("acl_openstack", "Group", "AddUserToGroup", ex);
                return "error";
            }
        }

        /// <summary>
        /// Removes a user from an existing group in OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public async Task<string> RemoveUserFromGroup(string token, string groupId, string userId)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(userId))
                return "error";

            try
            {
                if (_httpClient.DefaultRequestHeaders.Contains("X-Auth-Token"))
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                var response = await _httpClient.DeleteAsync($"{_apiUrl}/v3/groups/{groupId}/users/{userId}");

                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "user_removed_from_group";
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("acl_openstack", "Group", "RemoveUserFromGroup", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes an existing group in OpenStack.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="groupId">The ID of the group to delete.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public async Task<string> DeleteGroup(string token, string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
                return "error";

            try
            {
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                var response = await _httpClient.DeleteAsync($"{_apiUrl}/v3/groups/{groupId}");

                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "group_deleted";
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("acl_openstack", "Group", "DeleteGroup", ex);
                return "error";
            }
        }
    }
}
