using Newtonsoft.Json;

namespace acl_openstack_identity.ResourceTypes
{
    public class GroupTypes
    {
        public class OpenstackGroup
        {
            [JsonProperty("description")]
            public string description { get; set; }

            [JsonProperty("domain_id")]
            public string domainId { get; set; }

            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("links")]
            public GroupLinks links { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class GroupLinks
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
