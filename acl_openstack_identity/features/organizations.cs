using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using acl_openstack_identity.Resources;
using Npgsql;

namespace acl_openstack_identity.features
{
    public class organizations
    {
        private readonly IConfiguration _configuration;
        private OpenstackContext _context;
        private OpenstackClient _client;
        private readonly HttpClient _httpClient;
        private roles _roles;

        public organizations (IConfiguration configuration, OpenstackContext context, HttpClient httpClient)
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
            _roles = new roles(context);
        }

        /// <summary>
        /// Creates a new organization by interacting with OpenStack services to create a project, group, and assign roles.
        /// </summary>
        /// <param name="organization">An object containing the details of the organization to be created, including its name and description.</param>
        /// <returns>
        /// An <see cref="AddOrganizationObResult"/> object indicating the result of the operation:
        /// - "no_name" if the organization name is missing.
        /// - "error" if any error occurs during the creation process.
        /// - "organization_created" if the organization is successfully created, along with its ID.
        /// </returns>
        public async Task<AddOrganizationObResult> CreateOrganization(AddOrganizationOb organization)
        {
            // Check if the organization name is null or empty; return "no_name" if it is.
            if (string.IsNullOrEmpty(organization.name))
                return new AddOrganizationObResult { result = "no_name" };

            try
            {
                // Authenticate with the client to obtain a token for OpenStack operations.
                var token = await _client.Authenticate();

                // Initialize controllers for interacting with OpenStack resources.
                acl_openstack_identity.Resources.Project projectController = new acl_openstack_identity.Resources.Project(token.identityUrl, _httpClient);
                acl_openstack_identity.Resources.Group groupController = new acl_openstack_identity.Resources.Group(token.identityUrl, _httpClient);
                acl_openstack_identity.Resources.Role roleController = new acl_openstack_identity.Resources.Role(_httpClient, token.identityUrl);
                acl_openstack_identity.Resources.inheritRole inheritRoleController = new acl_openstack_identity.Resources.inheritRole(_httpClient, token.identityUrl);

                // Create a new OpenStack project for the organization.
                var project = await projectController.CreateProject(token.token, new ReourcesRequestsObjects.ProjectRequestOb
                {
                    name = organization.name,
                    is_domain = false,
                    description = organization.description,
                    domain_id = _configuration["OpenStack:Domain"], // Retrieve domain ID from configuration.
                    enabled = true,
                    parent_id = "0371cdb4db5c4787b882750144616f18" // Use a predefined parent ID.
                });

                // Check if project creation failed; return "error" if it did.
                if (project.Name == "error")
                    return new AddOrganizationObResult { result = "error" };

                // Create a new group for the organization.
                var organizationGroup = await groupController.AddGroup(token.token, new ReourcesRequestsObjects.GroupRequest.AddGroupRequest
                {
                    name = $"{organization.name}-users",
                    description = $"{organization.name} group members",
                    domainId = _configuration["OpenStack:Domain"]
                });

                // If group creation fails, delete the previously created project and return the error result.
                if (organizationGroup.name.Contains("error"))
                {
                    await projectController.DeleteProject(token.token, project.Id);
                    return new AddOrganizationObResult { result = organizationGroup.name };
                }

                // Assign a role to the group within the project.
                var assignRoleToMain = await roleController.AssignRoleToGroup(token.token, project.Id, organizationGroup.id, "11227907f18940609771d43e783eb494");

                // If role assignment fails, clean up by deleting the group and project, and return "error".
                if (assignRoleToMain == "error")
                {
                    await groupController.DeleteGroup(token.token, organizationGroup.id);
                    await projectController.DeleteProject(token.token, project.Id);
                    return new AddOrganizationObResult { result = "error" };
                }

                // Assign an inherited role to the group in the project.
                var assigninheritRole = await inheritRoleController.AssignInheritRoleToGroupInProject(token.token, project.Id, organizationGroup.id, "11227907f18940609771d43e783eb494");

                // If inherited role assignment fails, clean up and return "error".
                if (assigninheritRole == "error")
                {
                    await groupController.DeleteGroup(token.token, organizationGroup.id);
                    await projectController.DeleteProject(token.token, project.Id);
                    return new AddOrganizationObResult { result = "error" };
                }

                // Add the new organization to the local database context.
                var newOrganization = _context.Projects.Add(new Models.Project
                {
                    Name = organization.name,
                    OpenstackProjectId = project.Id,
                    Groupid = organizationGroup.id,
                    DescriptionText = organization.description
                });

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "organization_created" if the operation is successful, including the new organization's ID.
                return new AddOrganizationObResult { result = "organization_created", organizationId = (int)newOrganization.Entity.Id };
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "organizations", "CreateOrganization", ex);
                return new AddOrganizationObResult { result = "error" };
            }
        }

        /// <summary>
        /// Adds an owner to an organization by assigning the user to the organization's group and updating the database.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to which the owner will be added.</param>
        /// <param name="userId">The ID of the user to be added as an owner to the organization.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the organization or user is not found, or an exception occurs.
        /// - "user_added" if the user is successfully added as an owner to the organization.
        /// </returns>
        public async Task<string> AddOwnerToOrganisation(int organizationId, int userId)
        {
            try
            {
                // Retrieve the organization from the database using the provided organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                // If the organization is not found, return "error".
                if (organization == null)
                    return "error";

                // Retrieve the user from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                // Authenticate with the client to obtain a token for OpenStack operations.
                var token = await _client.Authenticate();

                // Add the "Owner" role to the organization using the roles library.
                var createOwner = await _roles.AddOwnerRole("Organization", (int)organization.Id);

                if (createOwner == "error")
                    return "cannot_create_owner_role";

                var ownerRole = _context.Roles.FirstOrDefault(r => r.Name.ToLower() == "owner" && r.ProjectId == organization.Id);

                if (ownerRole == null)
                    return "cannot_get_owner_role";

                _context.UsersRoles.Add(new Models.UsersRole
                {
                    UserId = user.Id,
                    RoleId = ownerRole.Id
                });

                // Initialize the Group controller for interacting with OpenStack group resources.
                Group group = new Group(token.identityUrl, _httpClient);

                // Add the user to the organization's group in OpenStack.
                string result = await group.AddUserToGroup(token.token, organization.Groupid, user.OpenstackId);

                // If adding the user to the group fails, return "error".
                if (result == "error")
                    return "error";

                // Add the user to the local ProjectsUsers association in the database.
                _context.ProjectsUsers.Add(new Models.ProjectsUser
                {
                    UserId = user.Id,
                    ProjectId = organization.Id
                });

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "user_added" if the operation is successful.
                return "user_added";
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "organizations", "AddOwnerToOrganisation", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes an organization from the database and associated OpenStack resources, including the project and group.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to be deleted. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the organization ID is invalid, the organization is not found, or an exception occurs.
        /// - "resources_exists" if the organization has existing child resources.
        /// - "cannot_delete_project" if the OpenStack project deletion fails.
        /// - "cannot_delete_group" if the OpenStack group deletion fails.
        /// - "organization_deleted" if the organization and its associated resources are successfully deleted.
        /// </returns>
        public async Task<string> DeleteOrganization(int organizationId = -1)
        {
            // Check if the organization ID is invalid; return "error" if it is.
            if (organizationId == -1)
                return "error";

            try
            {
                // Retrieve the organization from the database using the provided organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                // If the organization is not found, return "error".
                if (organization == null)
                    return "error";

                // Check if the organization has any existing child resources (sub-projects).
                var checkExistingResources = _context.Projects.Where(p => p.ParentId == organization.Id).ToList();

                // If any child resources exist, return "resources_exists".
                if (checkExistingResources.Any())
                    return "resources_exists";

                // Authenticate with the client to obtain a token for OpenStack operations.
                var token = await _client.Authenticate();

                // Initialize the Project controller for interacting with OpenStack project resources.
                Project project = new Project(token.identityUrl, _httpClient);

                // Attempt to delete the OpenStack project associated with the organization.
                var deleteProjectResult = await project.DeleteProject(token.token, organization.OpenstackProjectId);

                // If project deletion fails, return "cannot_delete_project".
                if (deleteProjectResult == "error")
                    return "cannot_delete_project";

                // Initialize the Group controller for interacting with OpenStack group resources.
                Group group = new Group(token.identityUrl, _httpClient);

                // Attempt to delete the OpenStack group associated with the organization.
                var deleteGroupResult = await group.DeleteGroup(token.token, organization.Groupid);

                // If group deletion fails, return "cannot_delete_group".
                if (deleteGroupResult == "error")
                    return "cannot_delete_group";

                // Remove all user associations with the organization in the local database.
                _context.ProjectsUsers.RemoveRange(_context.ProjectsUsers.Where(up => up.ProjectId == organization.Id));

                // Remove the organization from the local database.
                _context.Projects.Remove(organization);

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "organization_deleted" if the operation is successful.
                return "organization_deleted";
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "organizations", "DeleteOrganization", ex);
                return "error";
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
