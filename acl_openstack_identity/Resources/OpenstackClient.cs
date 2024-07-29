using acl_openstack_identity.Helpers;
using Newtonsoft.Json.Linq;
using System.Text;

namespace acl_openstack_identity.Resources
{
    public class OpenstackClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _authUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly string _domain;
        private readonly string _project;

        public OpenstackClient(string authUrl, string username, string password, string domain, string project)
        {
            _httpClient = new HttpClient();
            _authUrl = authUrl;
            _username = username;
            _password = password;
            _domain = domain;
            _project = project;
        }

        public async Task<(string token, string identityUrl)> Authenticate()
        {
            var authPayload = new
            {
                auth = new
                {
                    identity = new
                    {
                        methods = new[] { "password" },
                        password = new
                        {
                            user = new
                            {
                                name = _username,
                                domain = new { name = _domain },
                                password = _password
                            }
                        }
                    },
                    scope = new
                    {
                        project = new
                        {
                            domain = new
                            {
                                id = _domain
                            },
                            name = _project
                        }
                    }
                }
            };

            var content = new StringContent(JObject.FromObject(authPayload).ToString(), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_authUrl}/v3/auth/tokens", content);

                if (!response.IsSuccessStatusCode)
                    return ("error", null);

                string token = response.Headers.GetValues("X-Subject-Token").First();

                var responseBody = await response.Content.ReadAsStringAsync();

                var serviceCatalog = JObject.Parse(responseBody)["token"]["catalog"];

                var service = serviceCatalog.FirstOrDefault(service => service["type"].ToString() == "identity");

                if (service == null)
                    return ("error", null);

                var serviceUrl = service["endpoints"].FirstOrDefault(endpoint => endpoint["interface"].ToString() == "public")?["url"].ToString();

                if (serviceUrl == null)
                    return ("error", null);

                return (token, serviceUrl);
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("acl_openstack", "OpenstackClient", "Authenticate", ex);
                return ("error", null);
            }
        }
    }
}
