using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace acl_openstack_identity.ReourcesRequestsObjects
{
    public class ProjectRequestOb
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("is_domain")]
        public bool? is_domain { get; set; } = false;

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("domain_id")]
        public string domain_id { get; set; }

        [JsonProperty("enabled")]
        public bool? enabled { get; set; } = true;

        [JsonProperty("parent_id")]
        public string parent_id { get; set; }

        [JsonProperty("tags")]
        public string[]? tags { get; set; }

        [JsonProperty("options")]
        public object? options { get; set; }
    }

    public class ProjectEditRequestOb
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }

        [JsonPropertyName("name")]
        public string? name { get; set; }

        [JsonPropertyName("description")]
        public string? description { get; set; }
    }
}
