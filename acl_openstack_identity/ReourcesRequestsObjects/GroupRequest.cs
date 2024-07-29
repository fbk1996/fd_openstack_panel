using Newtonsoft.Json;

namespace acl_openstack_identity.ReourcesRequestsObjects
{
    public class GroupRequest
    {
        public class AddGroupRequest
        {
            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("description")]
            public string? description { get; set; }

            [JsonProperty("domain_id")]
            public string? domainId { get; set; }
        }

        public class EditGroupRequest
        {
            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("name")]
            public string? name { get; set; }

            [JsonProperty("description")]
            public string? description { get; set; }
        }
    }
}
