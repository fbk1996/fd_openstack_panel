using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace acl_openstack_identity.ReourcesRequestsObjects
{
    public class UserRequest
    {
        public class UserRequestAdd
        {
            [JsonProperty("default_project_id")]
            public string? defaultProjectId { get; set; }

            [JsonProperty("domain_id")]
            public string? domain_id { get; set; }

            [JsonProperty("enabled")]
            public bool? enabled { get; set; }

            [JsonProperty("name")]
            public string? name { get; set; }

            [JsonProperty("password")]
            public string? password { get; set; }
        }

        public class UserRequestEdit
        {
            [JsonProperty("id")]
            public string? id { get; set; }

            [JsonProperty("enabled")]
            public bool? enabled { get; set; }

            [JsonProperty("name")]
            public string? name { get; set; }
        }
    }
}
