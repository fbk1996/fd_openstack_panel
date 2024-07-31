using static acl_openstack_identity.ResourceTypes.ProjectTypes;
using acl_openstack_identity.ReourcesRequestsObjects;
using Newtonsoft.Json;
using System.Text;
using acl_openstack_identity.Helpers;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace acl_openstack_identity.Resources
{
    public class Project
    {
        private readonly HttpClient _httpClient; // HttpClient instance to make HTTP requests
        private readonly string _apiUrl; // Base URL of the API

        // Constructor to initialize the Project class with the API URL and domain
        public Project(string apiUrl, HttpClient httpClient)
        {
            _apiUrl = apiUrl;
            _httpClient = httpClient;
        }

        // Method to create a new project with the given token and project request object
        public async Task<OpenStackProject> CreateProject(string token, ProjectRequestOb requestOb)
        {
            try
            {
                // Create a dynamic object to hold the project properties
                dynamic projectOb = new ExpandoObject();

                // Conditionally add properties to the project object based on the request object
                if (!string.IsNullOrEmpty(requestOb.name))
                    projectOb.name = requestOb.name;
                if (!string.IsNullOrEmpty(requestOb.description))
                    projectOb.description = requestOb.description;
                if (!string.IsNullOrEmpty(requestOb.domain_id))
                    projectOb.domain_id = requestOb.domain_id;
                if (requestOb.enabled != null)
                    projectOb.enabled = requestOb.enabled;
                if (requestOb.is_domain != null)
                    projectOb.is_domain = requestOb.is_domain;
                if (!string.IsNullOrEmpty(requestOb.parent_id))
                    projectOb.parent_id = requestOb.parent_id;
                if (requestOb.tags != null && requestOb.tags.Length != 0)
                    projectOb.tags = requestOb.tags;
                if (requestOb.options != null)
                    projectOb.options = requestOb.options;

                // Create the payload for the request
                var projectPayload = new
                {
                    project = projectOb
                };

                // Serialize the payload to JSON
                var content = new StringContent(JObject.FromObject(projectPayload).ToString(), Encoding.UTF8, "application/json");

                if (_httpClient.DefaultRequestHeaders.Contains("X-Auth-Token"))
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                // Add the authorization token to the request headers
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);

                // Make the POST request to create the project
                var response = await _httpClient.PostAsync($"{_apiUrl}/v3/projects", content);

                // Handle the response
                if (!response.IsSuccessStatusCode)
                {
                    return new OpenStackProject { Name = "error " };
                }

                // Parse the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                var projectResponse = JObject.Parse(responseContent)["project"];
                var project = projectResponse.ToObject<OpenStackProject>();

                // Return the created project or an error object if parsing failed
                return project ?? new OpenStackProject { Name = "error project" };
            }
            catch (Exception ex)
            {
                // Log the exception and return an error object
                Logger.SendNormalException("acl_openstack", "Project", "CreateProject", ex);
                return new OpenStackProject { Name = "error" };
            }
        }

        // Method to edit an existing project with the given token and project edit request object
        public async Task<string> EditProject(string token, ProjectEditRequestOb requestOb)
        {
            try
            {
                // Create a dynamic object to hold the project properties
                dynamic projectOb = new ExpandoObject();

                // Conditionally add properties to the project object based on the request object
                if (!string.IsNullOrEmpty(requestOb.name))
                    projectOb.name = requestOb.name;
                if (!string.IsNullOrEmpty(requestOb.description))
                    projectOb.description = requestOb.description;

                // Create the payload for the request
                var requestBody = new
                {
                    project = projectOb
                };

                // Serialize the payload to JSON
                var content = new StringContent(JObject.FromObject(requestBody).ToString(), Encoding.UTF8, "application/json");

                if (_httpClient.DefaultRequestHeaders.Contains("X-Auth-Token"))
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                // Add the authorization token to the request headers
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);

                // Make the PATCH request to edit the project
                var response = await _httpClient.PatchAsync($"{_apiUrl}/v3/projects/{requestOb.id}", content);

                // Handle the response
                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "project_edited";
            }
            catch (Exception ex)
            {
                // Log the exception and return an error message
                Logger.SendNormalException("acl_openstack", "Project", "EditProject", ex);
                return "error";
            }
        }

        // Method to delete an existing project with the given token and project ID
        public async Task<string> DeleteProject(string token, string id)
        {
            // Return an error if the project ID is null or empty
            if (string.IsNullOrEmpty(id))
                return "error";

            try
            {
                if (_httpClient.DefaultRequestHeaders.Contains("X-Auth-Token"))
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                // Add the authorization token to the request headers
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);

                // Make the DELETE request to delete the project
                var response = await _httpClient.DeleteAsync($"{_apiUrl}/v3/projects/{id}");

                // Handle the response
                if (!response.IsSuccessStatusCode)
                    return "error";
                else
                    return "project_deleted";
            }
            catch (Exception ex)
            {
                // Log the exception and return an error message
                Logger.SendNormalException("acl_openstack", "Project", "DeleteProject", ex);
                return "error";
            }
        }
    }
}
