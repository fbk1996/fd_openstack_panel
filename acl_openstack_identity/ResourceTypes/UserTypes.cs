using Newtonsoft.Json;

namespace acl_openstack_identity.ResourceTypes
{
    public class UserTypes
    {
        public class OpenstackUser
        {
            [JsonProperty("default_project_id")]
            public string defaultProjectId { get; set; }

            [JsonProperty("domain_id")]
            public string domainId { get; set; }

            [JsonProperty("enabled")]
            public bool enabled { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("links")]
            public UserLinks links { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class UserLinks
        {
            [JsonProperty("self")]
            public string Self { get; set; }

            [JsonProperty("previous")]
            public string Previuos { get; set; }

            [JsonProperty("next")]
            public string Next { get; set; }
        }
    }
}
