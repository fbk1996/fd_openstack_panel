using Newtonsoft.Json;

namespace acl_openstack_identity.ResourceTypes
{
    public class RegionTypes
    {
        public class OpenstackRegions
        {
            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("links")]
            public RegionLinks Links { get; set; }

            [JsonProperty("parent_region_id")]
            public string ParentRegionId { get; set; }
        }

        public class RegionLinks
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
