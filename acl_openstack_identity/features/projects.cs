using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using Npgsql;

namespace acl_openstack_identity.features
{
    public class projects
    {
        private readonly OpenstackContext _context;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;
        private users _users;

        public projects (OpenstackContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _users = new users(context, configuration, httpClient);
            _httpClient = httpClient;
        }

        public projectDetailsOb GetProjectDetails(int projectId = -1)
        {
            if (projectId == -1)
                return new projectDetailsOb { id = -1 };

            try
            {

            }
            catch (NpgsqlException ex)
            {
                Logger.SendException
            }
        }
    }

    public class projectDetailsOb
    {
        public int? id { get; set; }
        public projectLinksDetailOb? parentProject { get; set; }
        public string? name { get; set; }
        public string? openstackId { get; set; }
    }

    public class projectLinksDetailOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
    }
}
