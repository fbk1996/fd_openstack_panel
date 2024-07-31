namespace acl_openstack_identity.Resources
{
    using acl_openstack_identity.Helpers;
    using Newtonsoft.Json.Linq;
    using static acl_openstack_identity.ResourceTypes.RegionTypes;

    /// <summary>
    /// This class provides methods to interact with OpenStack regions.
    /// </summary>
    public class Region
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use for HTTP requests.</param>
        /// <param name="apiUrl">The base URL of the OpenStack API.</param>
        public Region(HttpClient httpClient, string apiUrl)
        {
            _httpClient = httpClient;
            _apiUrl = apiUrl;
        }

        /// <summary>
        /// Retrieves a list of regions from the OpenStack API.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <returns>An array of OpenstackRegions objects or null if the request fails.</returns>
        public async Task<OpenstackRegions[]> GetRegions(string token)
        {
            try
            {
                if (_httpClient.DefaultRequestHeaders.Contains("X-Auth-Token"))
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                // Add the authentication token to the request headers
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);

                // Send a GET request to the regions endpoint
                var response = await _httpClient.GetAsync($"{_apiUrl}/v3/regions");

                // Check if the response indicates success
                if (!response.IsSuccessStatusCode)
                    return null;

                // Read the response content as a string
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse the response content into a JObject
                var regionsResponse = JObject.Parse(responseContent)["regions"];

                // Deserialize the JSON array to an array of OpenstackRegions objects
                var regions = regionsResponse.ToObject<OpenstackRegions[]>();

                return regions ?? null;
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur
                Logger.SendNormalException("acl_openstack", "Region", "GetRegions", ex);
                return null;
            }
        }
    }
}
