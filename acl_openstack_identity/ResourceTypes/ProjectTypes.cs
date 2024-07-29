using Newtonsoft.Json;

namespace acl_openstack_identity.ResourceTypes
{
    public class ProjectTypes
    {
        public class OpenStackProject
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("domain_id")]
            public string DomainId { get; set; }

            [JsonProperty("enabled")]
            public bool Enabled { get; set; }

            [JsonProperty("is_domain")]
            public bool IsDomain { get; set; }

            [JsonProperty("parent_id")]
            public string ParentId { get; set; }

            [JsonProperty("links")]
            public ProjectLinks Links { get; set; }

            [JsonProperty("options")]
            public object Options { get; set; }
        }

        public class ProjectLinks
        {
            [JsonProperty("self")]
            public string Self { get; set; }

            [JsonProperty("previous")]
            public string Previous { get; set; }

            [JsonProperty("next")]
            public string Next { get; set; }
        }
    }
}
