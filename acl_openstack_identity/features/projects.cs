using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using acl_openstack_identity.Resources;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Retrieves detailed information about a specific project based on the provided project ID.
        /// </summary>
        /// <param name="projectId">The ID of the project whose details are to be retrieved. Defaults to -1.</param>
        /// <returns>
        /// A <see cref="projectDetailsOb"/> object containing the project's details, including its ID, name, OpenStack ID, and parent project details if applicable.
        /// - If the projectId is invalid, returns a projectDetailsOb with id = -1.
        /// - If an exception occurs or the project is not found, returns a projectDetailsOb with id = -1.
        /// </returns>
        public projectDetailsOb GetProjectDetails(int projectId = -1)
        {
            // Validate the project ID. Return a projectDetailsOb with id = -1 if the projectId is invalid.
            if (projectId == -1)
                return new projectDetailsOb { id = -1 };

            try
            {
                // Retrieve the project details from the database using the provided project ID.
                var project = (projectDetailsOb)_context.Projects
                    .AsNoTracking() // Ensure no tracking changes to the entity.
                    .Where(p => p.Id == projectId)
                    .Select(p => new projectDetailsOb
                    {
                        id = (int)p.Id,
                        // Retrieve parent project details if a parent project exists.
                        parentProject = (p.ParentId != null) ? new projectLinksDetailOb
                        {
                            id = (int)p.Parent.Id,
                            name = p.Parent.Name,
                            scope = scopeSelector.GetScopeString((int)p.Parent.Scope)
                        } : null,
                        name = p.Name,
                        openstackId = p.OpenstackProjectId,
                        description = p.DescriptionText
                    })
                    .FirstOrDefault();

                // Return the project details, or a default projectDetailsOb with id = -1 if the project is not found.
                return project ?? new projectDetailsOb { id = -1 };
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return a projectDetailsOb with id = -1.
                Logger.SendException("openstack_panel", "projects", "GetProjectDetails", ex);
                return new projectDetailsOb { id = -1 };
            }
        }

        /// <summary>
        /// Retrieves a list of users linked to a project and its parent projects. 
        /// The method checks whether each user is directly linked to the specified project.
        /// </summary>
        /// <param name="projectId">The ID of the project for which to retrieve linked users. Defaults to -1.</param>
        /// <returns>
        /// A list of <see cref="projectLinksOb"/> objects representing the users linked to the project.
        /// - If the projectId is invalid or the project is not found, returns null.
        /// - If an exception occurs, returns null.
        /// </returns>
        public List<projectLinksOb> GetProjectLinks(int projectId = -1)
        {
            // Validate the project ID. Return null if the projectId is invalid.
            if (projectId == -1)
                return null;

            try
            {
                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return null.
                if (project == null)
                    return null;

                // Initialize a list to hold the project IDs associated with the parent projects.
                List<int> projectIds = new List<int>();

                // Recursive function to retrieve parent projects and add their IDs to the projectIds list.
                void GetParentProjects(int parentId)
                {
                    if (parentId == null)
                        return;

                    // Retrieve the parent project from the database using the parent ID.
                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == parentId);

                    // If the parent project exists, add its ID to the list and call the function recursively.
                    if (parentProject != null)
                    {
                        projectIds.Add((int)parentProject.Id);
                        GetParentProjects((int)parentProject.ParentId);
                    }
                }

                // Populate the projectIds list with the current project's parent projects.
                GetParentProjects((int)project.ParentId);

                // Start with a queryable collection of users from the context.
                IQueryable<Models.User> query = _context.Users;

                // Filter the users based on their association with any of the projects in the projectIds list and where the project scope is 2.
                query = query.Where(u => u.ProjectsUsers.Any(up => projectIds.Contains((int)up.ProjectId) && up.Project.Scope == 2));

                // Project the filtered users into a list of projectLinksOb objects.
                var projectLinks = query
                    .AsNoTracking() // Ensure no tracking changes to the entities.
                    .Select(ul => new projectLinksOb
                    {
                        userId = (int)ul.Id,
                        name = (string)ul.Name,
                        lastname = (string)ul.Lastname,
                        // Check if the user is directly linked to the specified project.
                        isInProject = _context.ProjectsUsers.Any(up => up.ProjectId == projectId && up.UserId == ul.Id)
                    }).ToList();

                // Return the list of projectLinksOb objects.
                return projectLinks;
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return null.
                Logger.SendException("openstack_panel", "projects", "GetProjectLinks", ex);
                return null;
            }
        }

        /// <summary>
        /// Adds a new project to the system. The project is also created in OpenStack, and its details are stored in the local database.
        /// </summary>
        /// <param name="project">An object containing the details of the project to be added, including name, description, and parent project ID.</param>
        /// <param name="uid">The ID of the user creating the project. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the user ID is invalid, the project name is missing, the user is not found, or an exception occurs.
        /// - "no_name" if the project name is missing.
        /// - "exists" if a project with the same name already exists.
        /// - "project_created" if the project is successfully created.
        /// </returns>
        public async Task<string> AddProject(AddProjectOb project, int uid = -1)
        {
            // Validate the user ID. Return "error" if the uid is invalid.
            if (uid == -1)
                return "error";

            // Validate the project name. Return "no_name" if the project name is missing or empty.
            if (string.IsNullOrEmpty(project.name))
                return "no_name";

            try
            {
                // Retrieve the user from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);

                // If the user is not found, return "error".
                if (user == null)
                    return "error";

                // Check if a project with the same name already exists in the system.
                var checkProject = _context.Projects.FirstOrDefault(p => p.Name.ToLower() == project.name.ToLower().Trim());

                // If the project exists, return "exists".
                if (checkProject != null)
                    return "exists";

                // Retrieve the project associated with the user to determine the organization hierarchy.
                var projectDb = _context.Projects.FirstOrDefault(p => p.ProjectsUsers.Any(pu => pu.UserId == uid));

                // Initialize the organization ID with the current project's ID.
                int organizationId = (int)projectDb.Id;

                // Recursive function to determine the organization ID by checking the project's parent hierarchy.
                void CheckForOrganizationProject(int parentId, int currentId)
                {
                    if (parentId == null)
                    {
                        organizationId = currentId;
                        return;
                    }

                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == parentId);

                    if (parentProject != null)
                    {
                        organizationId = (int)parentProject.Id;
                        CheckForOrganizationProject((int)parentProject.ParentId, organizationId);
                    }
                }

                // Determine the organization ID by checking the current project's parent hierarchy.
                CheckForOrganizationProject((int)projectDb.ParentId, organizationId);

                // Retrieve the organization based on the determined organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                // If the organization is not found, return "error".
                if (organization == null)
                    return "error";

                // Retrieve the parent project from the database using the provided parent project ID.
                var parentProjectDb = _context.Projects.FirstOrDefault(p => p.Id == project.parentId);

                // Authenticate with OpenStack using the user's credentials.
                OpenstackClient _client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.Email,
                    user.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    organization.OpenstackProjectId
                );

                var openstackToken = await _client.Authenticate();

                // Create the project in OpenStack.
                var openstackProject = new Project(openstackToken.identityUrl, _httpClient);
                var createProjectResult = await openstackProject.CreateProject(openstackToken.token, new ReourcesRequestsObjects.ProjectRequestOb
                {
                    name = project.name.Trim(),
                    is_domain = false,
                    description = project.description.Trim(),
                    parent_id = parentProjectDb.OpenstackProjectId,
                    enabled = true,
                    domain_id = _configuration["OpenStack:Domain"]
                });

                // If there was an error creating the project in OpenStack, return "error".
                if (createProjectResult.Name == "error")
                    return "error";

                // Add the new project to the local database.
                _context.Projects.Add(new Models.Project
                {
                    Name = project.name.Trim(),
                    OpenstackProjectId = createProjectResult.Id,
                    DescriptionText = project.description.Trim(),
                    Scope = 2,
                    CreationDate = DateTime.Now,
                });

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "project_created" if the project is successfully created.
                return "project_created";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("openstack_panel", "projects", "AddProject", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits the details of an existing project in the system and updates the corresponding project in OpenStack.
        /// </summary>
        /// <param name="project">An object containing the new details of the project, including the new name, old name, and description.</param>
        /// <param name="uid">The ID of the user making the edit. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the user ID is invalid, the project name is missing, the user or organization is not found, or an exception occurs.
        /// - "no_name" if the project name or old name is missing.
        /// - "exists" if a project with the new name already exists and is different from the old name.
        /// - "project_edited" if the project is successfully edited.
        /// </returns>
        public async Task<string> EditProject(EditProjectOb project, int? uid = -1)
        {
            // Validate the user ID. Return "error" if the uid is invalid.
            if (uid == -1)
                return "error";

            // Validate the project name and old name. Return "no_name" if either is missing or empty.
            if (string.IsNullOrEmpty(project.name) || string.IsNullOrEmpty(project.oldName))
                return "no_name";

            try
            {
                // Retrieve the user from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);

                // If the user is not found, return "error".
                if (user == null)
                    return "error";

                // Check if a project with the new name already exists in the system.
                var checkIfExist = _context.Projects.FirstOrDefault(p => p.Name.ToLower() == project.name.ToLower().Trim());

                // If a different project with the same new name exists, return "exists".
                if (checkIfExist != null && project.name != project.oldName)
                    return "exists";

                // Retrieve the project from the database using the provided project ID.
                var projectDb = _context.Projects.FirstOrDefault(p => p.Id == project.id);

                // Determine the organization ID by checking the project's parent hierarchy.
                int organizationId = (int)projectDb.Id;

                void CheckForOrganizationProject(int parentId, int currentId)
                {
                    if (parentId == null)
                    {
                        organizationId = currentId;
                        return;
                    }

                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == parentId);

                    if (parentProject != null)
                    {
                        organizationId = (int)parentProject.Id;
                        CheckForOrganizationProject((int)parentProject.ParentId, organizationId);
                    }
                }

                // Populate the organizationId with the correct value by checking the project's parent hierarchy.
                CheckForOrganizationProject((int)projectDb.ParentId, organizationId);

                // Retrieve the organization based on the determined organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                // If the organization is not found, return "error".
                if (organization == null)
                    return "error";

                // Authenticate with OpenStack using the user's credentials.
                OpenstackClient _client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.Email,
                    user.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    organization.OpenstackProjectId
                );

                var openstackToken = await _client.Authenticate();

                // Create a new Project object for OpenStack and attempt to edit the project.
                var openstackProject = new Project(openstackToken.identityUrl, _httpClient);

                var editProjectResult = await openstackProject.EditProject(openstackToken.token, new ReourcesRequestsObjects.ProjectEditRequestOb
                {
                    id = projectDb.OpenstackProjectId,
                    name = project.name,
                    description = project.description
                });

                // If the OpenStack project edit operation fails, return "error".
                if (editProjectResult == "error")
                    return "error";

                // Update the project details in the local database.
                projectDb.Name = project.name.Trim();
                projectDb.DescriptionText = project.description.Trim();

                // Save the changes to the database.
                await _context.SaveChangesAsync();

                // Return "project_edited" if the project is successfully edited.
                return "project_edited";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_panel", "projects", "EditProject", ex);
                return "error";
            }
        }

        /// <summary>
        /// Modifies the user links associated with a specific project. This includes adding new users to the project and removing existing users who are no longer linked.
        /// </summary>
        /// <param name="links">An object containing the project ID and the list of user IDs to be linked to the project.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the project ID is invalid or an exception occurs.
        /// - "links_modified" if the user links are successfully modified.
        /// </returns>
        public async Task<string> ModifyProjectLinks(ProjectModifyLinksOb links)
        {
            // Validate the project ID. Return "error" if the project ID is invalid.
            if (links.id == -1)
                return "error";

            try
            {
                // Retrieve the current list of user IDs associated with the specified project.
                var existingUsersInProjects = _context.ProjectsUsers
                    .Where(up => up.ProjectId == links.id)
                    .Select(up => (int?)up.UserId)
                    .ToList();

                // Convert the list of new user IDs from the input to nullable integers.
                var userIdsAsNullable = links.userIds.Select(id => (int?)id).ToList();

                // Determine which users need to be added by finding the difference between the desired and current user links.
                var usersToAdd = userIdsAsNullable.Except(existingUsersInProjects).ToList();

                // Add new user-project links for each user that needs to be added.
                foreach (var user in usersToAdd)
                {
                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        ProjectId = (long)links.id,
                        UserId = (long)user
                    });
                }

                // Determine which users need to be removed by finding the difference between the current and desired user links.
                var usersToDelete = existingUsersInProjects.Where(up => !userIdsAsNullable.Contains(up)).ToList();

                // Remove existing user-project links for each user that needs to be removed.
                foreach (var user in usersToDelete)
                {
                    _context.ProjectsUsers.Remove(_context.ProjectsUsers.FirstOrDefault(up => up.UserId == user && up.ProjectId == links.id));
                }

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "links_modified" if the operation is successful.
                return "links_modified";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_panel", "projects", "ModifyProjectLinks", ex);
                return "error";
            }
        }

        /// <summary>
        /// Adds a new user to the system and associates the user with a specified project.
        /// The method first creates the user and then links the user to the given project.
        /// </summary>
        /// <param name="user">An object containing the details of the user to be added.</param>
        /// <param name="projectId">The ID of the project to which the user should be linked. Defaults to -1.</param>
        /// <param name="uid">The ID of the user performing the operation. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the project ID or user ID is invalid, or if an exception occurs.
        /// - "user_created" if the user is successfully created and linked to the project.
        /// </returns>
        public async Task<string> AddUser(addUserOb user, int projectId = -1, int uid = -1)
        {
            // Validate the project ID and the user ID. Return "error" if either is invalid.
            if (projectId == -1 || uid == -1)
                return "error";

            try
            {
                // Attempt to add the user by calling the AddUser method from the _users service.
                var userResult = await _users.AddUser(user, uid);

                // If the user creation fails, return "error".
                if (userResult.result != "user_created")
                    return userResult.result;

                // If the user creation is successful, associate the user with the specified project.
                _context.ProjectsUsers.Add(new Models.ProjectsUser
                {
                    ProjectId = (long)projectId,
                    UserId = (long)userResult.id
                });

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "user_created" if the operation is successful.
                return "user_created";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "projects", "AddUser", ex);
                return "error";
            }
        }

        /// <summary>
        /// Changes the parent project of a specified project in the system.
        /// This method updates the parent project ID of the given project to a new parent project ID.
        /// </summary>
        /// <param name="projectId">The ID of the project whose parent is to be changed. Defaults to -1.</param>
        /// <param name="parentId">The new parent project ID to be set. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the project ID or parent ID is invalid, the project is not found, or an exception occurs.
        /// - "parent_changed" if the parent project is successfully updated.
        /// </returns>
        public async Task<string> ChangeParent(int projectId = -1, int parentId = -1)
        {
            // Validate the project ID and parent ID. Return "error" if either is invalid.
            if (projectId == -1 && parentId == -1)
                return "error";

            try
            {
                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return "error".
                if (project == null)
                    return "error";

                // Update the parent ID of the project.
                project.ParentId = parentId;

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "parent_changed" if the operation is successful.
                return "parent_changed";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "projects", "ChangeParent", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes a specified project from the system and OpenStack. 
        /// The method first authenticates the user, attempts to delete the project in OpenStack, 
        /// and then removes the project and its associated user links from the local database.
        /// </summary>
        /// <param name="projectId">The ID of the project to be deleted. Defaults to -1.</param>
        /// <param name="uid">The ID of the user performing the deletion. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the project ID or user ID is invalid, the project or user is not found, or an exception occurs.
        /// - "project_deleted" if the project is successfully deleted from both OpenStack and the local database.
        /// </returns>
        public async Task<string> DeleteProject(int projectId = -1, int uid = -1)
        {
            // Validate the project ID and user ID. Return "error" if either is invalid.
            if (projectId == -1 && uid == -1)
                return "error";

            try
            {
                // Retrieve the user from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);

                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project or user is not found, return "error".
                if (project == null || user == null)
                    return "error";

                // Create an OpenStack client for the authenticated user.
                var client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.Email,
                    user.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    project.OpenstackProjectId
                );

                // Authenticate with OpenStack and get the token.
                var openstackToken = await client.Authenticate();

                // Create a new Project object for OpenStack and attempt to delete the project.
                var openstackProject = new Project(openstackToken.identityUrl, _httpClient);
                var deleteProjectResult = await openstackProject.DeleteProject(openstackToken.token, project.OpenstackProjectId);

                // If the OpenStack project deletion fails, return "error".
                if (deleteProjectResult == "error")
                    return "error";

                // Remove all user-project links associated with the project from the local database.
                _context.ProjectsUsers.RemoveRange(_context.ProjectsUsers.Where(p => p.ProjectId == projectId));

                // Remove the project from the local database.
                _context.Projects.Remove(project);

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "project_deleted" if the operation is successful.
                return "project_deleted";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "projects", "DeleteProject", ex);
                return "error";
            }
        }
    }

    public class projectDetailsOb
    {
        public int? id { get; set; }
        public projectLinksDetailOb? parentProject { get; set; }
        public string? name { get; set; }
        public string? openstackId { get; set; }
        public string? description { get; set; }
    }

    public class projectLinksDetailOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? scope { get; set; }
    }

    public class projectLinksOb
    {
        public int? userId { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public bool? isInProject { get; set; }
    }

    public class AddProjectOb
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public int? parentId { get; set; }
    }

    public class EditProjectOb
    {
        public int? id { get; set; } = -1;
        public string? name { get; set; }
        public string? oldName { get; set; }
        public string? description { get; set; }
    }

    public class ProjectModifyLinksOb
    {
        public int? id { get; set; } = -1;
        public List<int>? userIds { get; set; }
    }
}
