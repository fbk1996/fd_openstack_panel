using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using acl_openstack_identity.Resources;
using Npgsql;

namespace acl_openstack_identity.features
{
    public class organizations
    {
        private readonly IConfiguration _configuration;
        private assecoOpenstackContext _context;
        private OpenstackClient _client;
        private readonly HttpClient _httpClient;

        public organizations (IConfiguration configuration, assecoOpenstackContext context, HttpClient httpClient)
        {
            _configuration = configuration;
            _context = context;
            _client = new OpenstackClient(
                _configuration["OpenStack:AuthUrl"],
                _configuration["OpenStack:Username"],
                _configuration["Openstack:Password"],
                _configuration["OpenStack:Domain"],
                _configuration["OpenStack:Project"]
                );
            _httpClient = httpClient;
        }

        public async Task<AddOrganizationObResult> CreateOrganization(AddOrganizationOb organization)
        {
            if (string.IsNullOrEmpty(organization.name))
                return new AddOrganizationObResult { result = "no_name" };

            try
            {
                var token = await _client.Authenticate();
                acl_openstack_identity.Resources.Project projectController = new acl_openstack_identity.Resources.Project(token.identityUrl, _httpClient);
                acl_openstack_identity.Resources.Group groupController = new acl_openstack_identity.Resources.Group(token.identityUrl, _httpClient);
                acl_openstack_identity.Resources.Role roleController = new acl_openstack_identity.Resources.Role(_httpClient, token.identityUrl);
                acl_openstack_identity.Resources.inheritRole inheritRoleController = new acl_openstack_identity.Resources.inheritRole(_httpClient, token.identityUrl);
                var project = await projectController.CreateProject(token.token, new ReourcesRequestsObjects.ProjectRequestOb
                {
                    name = organization.name,
                    is_domain = false,
                    description = organization.description,
                    domain_id = _configuration["OpenStack:Domain"],
                    enabled = true,
                    parent_id = "0371cdb4db5c4787b882750144616f18"
                });

                if (project.Name == "error")
                    return new AddOrganizationObResult { result = "error" };

                //create organization group
                var organizationGroup = await groupController.AddGroup(token.token, new ReourcesRequestsObjects.GroupRequest.AddGroupRequest
                {
                    name = $"{organization.name}-users",
                    description = $"{organization.name} group members",
                    domainId = _configuration["OpenStack:Domain"]
                });

                if (organizationGroup.name.Contains("error"))
                {

                    await projectController.DeleteProject(token.token, project.Id);

                    return new AddOrganizationObResult { result = organizationGroup.name };
                }

                var assignRoleToMain = await roleController.AssignRoleToGroup(token.token, project.Id, organizationGroup.id, "11227907f18940609771d43e783eb494");

                if (assignRoleToMain == "error")
                {
                    await groupController.DeleteGroup(token.token, organizationGroup.id);
                    await projectController.DeleteProject(token.token, project.Id);
                    return new AddOrganizationObResult { result = "error" };
                }

                var assigninheritRole = await inheritRoleController.AssignInheritRoleToGroupInProject(token.token, project.Id, organizationGroup.id, "11227907f18940609771d43e783eb494");

                if (assigninheritRole == "error")
                {
                    await groupController.DeleteGroup(token.token, organizationGroup.id);
                    await projectController.DeleteProject(token.token, project.Id);
                    return new AddOrganizationObResult { result = "error" };
                }

                var newOrganization = _context.Organisations.Add(new Models.Organisation
                {
                    Name = organization.name, // Ensure proper type conversion if necessary
                    OpenstackProjectId = project.Id,
                    GroupId = organizationGroup.id
                });

                await _context.SaveChangesAsync();

                return new AddOrganizationObResult { result = "organization_created", organizationId = (int)newOrganization.Entity.Id };
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "organizations", "CreateOrganization", ex);
                return new AddOrganizationObResult { result = "error" };
            }
        }
    }

    public class AddOrganizationOb
    {
        public string? name { get; set; }
        public string? description { get; set; }
    }

    public class AddOrganizationObResult
    {
        public string? result { get; set; }
        public int? organizationId { get; set; }
    }
}
