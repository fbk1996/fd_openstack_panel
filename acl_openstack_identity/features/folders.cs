using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using acl_openstack_identity.Resources;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace acl_openstack_identity.features
{
    public class folders
    {
        private readonly OpenstackContext _context;
        private readonly IConfiguration _configuration;
        private users _users;
        private HttpClient _httpClient;

        public folders( OpenstackContext context, IConfiguration configuration, HttpClient client)
        {
            _context = context;
            _configuration = configuration;
            _users = new users(context, configuration, client);
            _httpClient = client;
        }

        /// <summary>
        /// Retrieves detailed information about a specific folder (project) based on the provided folder ID.
        /// The method returns details such as the folder's ID, name, OpenStack ID, description, and its parent folder's details.
        /// </summary>
        /// <param name="folderId">The ID of the folder whose details are to be retrieved. Defaults to -1.</param>
        /// <returns>
        /// A <see cref="folderDetailsOb"/> object containing the folder's details, or null if the folder ID is invalid, the folder is not found, or an exception occurs.
        /// </returns>
        public folderDetailsOb GetFolderDetails(int folderId = -1)
        {
            // Validate the folder ID. Return null if the folder ID is invalid.
            if (folderId == -1)
                return null;

            try
            {
                // Retrieve the folder details from the database using the provided folder ID.
                var folder = (folderDetailsOb)_context.Projects
                    .AsNoTracking() // Ensure no tracking changes to the entity.
                    .Where(p => p.Id == folderId)
                    .Select(p => new folderDetailsOb
                    {
                        id = (int)p.Id,
                        name = p.Name,
                        openstackId = p.OpenstackProjectId,
                        description = p.DescriptionText,
                        // Retrieve parent folder details if a parent folder exists.
                        parent = (p.ParentId != null) ? new folderParentDetailOb
                        {
                            id = (int)p.Parent.Id,
                            name = p.Parent.Name,
                            scope = scopeSelector.GetScopeString((int)p.Parent.Scope)
                        } : null
                    })
                    .FirstOrDefault();

                // Return the folder details, or null if the folder is not found.
                return folder ?? null;
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return null.
                Logger.SendException("Openstack_Panel", "folders", "GetFolderDetails", ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of users linked to a folder (project) and its parent folders. 
        /// The method checks whether each user is directly linked to the specified folder.
        /// </summary>
        /// <param name="folderId">The ID of the folder for which to retrieve linked users. Defaults to -1.</param>
        /// <returns>
        /// A list of <see cref="folderLinksOb"/> objects representing the users linked to the folder.
        /// - If the folderId is invalid or the folder is not found, returns null.
        /// - If an exception occurs, returns null.
        /// </returns>
        public List<folderLinksOb> GetFolderLinks(int folderId = -1)
        {
            // Validate the folder ID. Return null if the folder ID is invalid.
            if (folderId == -1)
                return null;

            try
            {
                // Retrieve the folder from the database using the provided folder ID.
                var folder = _context.Projects.FirstOrDefault(p => p.Id == folderId);

                // If the folder is not found, return null.
                if (folder == null)
                    return null;

                // Initialize a list to hold the project IDs associated with the parent folders.
                List<int> projectIds = new List<int>();

                // Recursive function to retrieve parent folders and add their IDs to the projectIds list.
                void GetParentProjects(int parentId)
                {
                    if (parentId == null)
                        return;

                    // Retrieve the parent folder from the database using the parent ID.
                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == parentId);

                    // If the parent folder exists, add its ID to the list and call the function recursively.
                    if (parentProject != null)
                    {
                        projectIds.Add((int)parentProject.Id);
                        GetParentProjects((int)parentProject.ParentId);
                    }
                }

                // Populate the projectIds list with the current folder's parent folders.
                GetParentProjects((int)folder.ParentId);

                // Start with a queryable collection of users from the context.
                IQueryable<Models.User> query = _context.Users;

                // Filter the users based on their association with any of the folders in the projectIds list and where the project scope is 2.
                query = query.Where(u => u.ProjectsUsers.Any(up => projectIds.Contains((int)up.ProjectId) && up.Project.Scope == 2));

                // Project the filtered users into a list of folderLinksOb objects.
                var folderLinks = query
                    .AsNoTracking() // Ensure no tracking changes to the entities.
                    .Select(ul => new folderLinksOb
                    {
                        userId = (int)ul.Id,
                        name = ul.Name,
                        lastname = ul.Lastname,
                        // Check if the user is directly linked to the specified folder.
                        isInProject = _context.ProjectsUsers.Any(up => up.ProjectId == folderId && up.UserId == ul.Id)
                    }).ToList();

                // Return the list of folderLinksOb objects.
                return folderLinks;
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return null.
                Logger.SendException("Openstack_Panel", "folders", "GetFolderLinks", ex);
                return null;
            }
        }

        /// <summary>
        /// Adds a new folder (project) to the system under a specified parent project. 
        /// The method creates the folder in both the local database and in OpenStack.
        /// </summary>
        /// <param name="folder">An object containing the details of the folder to be added, including name, description, and parent project ID.</param>
        /// <param name="uid">The ID of the user performing the operation. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the user ID is invalid, the folder name is missing, the parent project is not found, or an exception occurs.
        /// - "no_name" if the folder name is missing.
        /// - "exists" if a folder with the same name already exists under the same parent.
        /// - "folder_added" if the folder is successfully created.
        /// </returns>
        public async Task<string> AddFolder(AddFolderOb folder, int uid = -1)
        {
            // Validate the user ID. Return "error" if the uid is invalid.
            if (uid == -1)
                return "error";

            // Validate the folder name. Return "no_name" if the folder name is missing or empty.
            if (string.IsNullOrEmpty(folder.name))
                return "no_name";

            try
            {
                // Retrieve the user from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);

                // If the user is not found, return "error".
                if (user == null)
                    return "error";

                // Retrieve the parent project from the database using the provided parent project ID.
                var parentProject = _context.Projects.FirstOrDefault(p => p.Id == folder.parentId);

                // If the parent project is not found, return "error".
                if (parentProject == null)
                    return "error";

                // Determine the organization ID by checking the project's parent hierarchy.
                int organizationId = (int)parentProject.Id;

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
                CheckForOrganizationProject((int)parentProject.ParentId, organizationId);

                // Retrieve the organization based on the determined organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                // If the organization is not found, return "error".
                if (organization == null)
                    return "error";

                // Initialize a list to hold the folder IDs.
                List<int> folderIds = new List<int>();

                // Recursive function to collect folder IDs associated with the current parent.
                async Task CollectFolderIds(int currentParentId)
                {
                    var items = await _context.Projects
                        .Where(p => p.ParentId == currentParentId)
                        .ToListAsync();

                    foreach (var item in items)
                    {
                        if (item.Scope == 1)
                        {
                            folderIds.Add((int)item.Id);
                            await CollectFolderIds((int)item.Id);
                        }
                    }
                }

                // Collect all folder IDs under the organization.
                await CollectFolderIds(organizationId);

                // Check if a folder with the same name already exists under the same parent.
                var checkFolder = _context.Projects.Where(p => folderIds.Contains((int)p.Id) && p.Scope == 1 && p.Name.ToLower() == folder.name.ToLower().Trim());

                // If a folder with the same name exists, return "exists".
                if (checkFolder != null)
                    return "exists";

                // Create an OpenStack client for the authenticated user.
                var client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.Email,
                    user.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    organization.OpenstackProjectId
                );

                // Authenticate with OpenStack and get the token.
                var openstackToken = await client.Authenticate();

                // Create a new Project object in OpenStack and attempt to create the folder.
                var openstackProject = new Project(openstackToken.identityUrl, _httpClient);

                var createOpenstackProjectResult = await openstackProject.CreateProject(openstackToken.token, new ReourcesRequestsObjects.ProjectRequestOb
                {
                    name = folder.name,
                    description = folder.description,
                    is_domain = false,
                    enabled = true,
                    domain_id = _configuration["OpenStack:Domain"],
                    parent_id = parentProject.OpenstackProjectId
                });

                // If the OpenStack project creation fails, return "error".
                if (createOpenstackProjectResult != null && createOpenstackProjectResult.Name == "error")
                    return "error";

                // Add the new folder to the local database.
                _context.Projects.Add(new Models.Project
                {
                    Name = folder.name.Trim(),
                    DescriptionText = folder.description.Trim(),
                    OpenstackProjectId = createOpenstackProjectResult.Id,
                    Scope = 1,
                    CreationDate = DateTime.Now
                });

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "folder_added" if the operation is successful.
                return "folder_added";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "folders", "AddFolder", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits the details of an existing folder (project) in both the local database and OpenStack.
        /// The method updates the folder's name and description.
        /// </summary>
        /// <param name="folder">An object containing the new details of the folder, including its ID, new name, old name, and description.</param>
        /// <param name="uid">The ID of the user performing the operation. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the user ID or folder ID is invalid, the folder or organization is not found, or an exception occurs.
        /// - "no_name" if the new name or old name of the folder is missing.
        /// - "exists" if a folder with the new name already exists under the same organization and the new name differs from the old name.
        /// - "folder_edited" if the folder is successfully edited.
        /// </returns>
        public async Task<string> EditFolder(EditFolderOb folder, int uid = -1)
        {
            // Validate the user ID and folder ID. Return "error" if either is invalid.
            if (uid == -1 || folder.id == -1)
                return "error";

            // Validate the folder names. Return "no_name" if either the new name or old name is missing or empty.
            if (string.IsNullOrEmpty(folder.name) || string.IsNullOrEmpty(folder.oldName))
                return "no_name";

            try
            {
                // Retrieve the user from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);

                // If the user is not found, return "error".
                if (user == null)
                    return "error";

                // Retrieve the folder from the database using the provided folder ID.
                var folderDb = _context.Projects.FirstOrDefault(p => p.Id == folder.id);

                // If the folder is not found, return "error".
                if (folderDb == null)
                    return "error";

                // Determine the organization ID by checking the folder's parent hierarchy.
                int organizationId = (int)folderDb.Id;

                void CheckForOrganizationProject(int? parentId, int currentId)
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

                // Determine the organization ID by checking the folder's parent hierarchy.
                CheckForOrganizationProject((int)folderDb.ParentId, organizationId);

                // Retrieve the organization based on the determined organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                // If the organization is not found, return "error".
                if (organization == null)
                    return "error";

                // Initialize a list to hold the folder IDs.
                List<int> folderIds = new List<int>();

                // Recursive function to collect folder IDs associated with the current parent.
                async Task CollectFolderIds(int currentParentId)
                {
                    var items = await _context.Projects
                        .Where(p => p.ParentId == currentParentId)
                        .ToListAsync();

                    foreach (var item in items)
                    {
                        if (item.Scope == 1)
                        {
                            folderIds.Add((int)item.Id);
                            await CollectFolderIds((int)item.Id);
                        }
                    }
                }

                // Collect all folder IDs under the organization.
                await CollectFolderIds(organizationId);

                // Check if a folder with the new name already exists under the same organization.
                var checkFolder = _context.Projects.FirstOrDefault(p => folderIds.Contains((int)p.Id) && p.Scope == 1 && p.Name.ToLower() == folder.name.ToLower().Trim());

                // If a folder with the same name exists and the new name differs from the old name, return "exists".
                if (checkFolder != null && folder.name != folder.oldName)
                    return "exists";

                // Create an OpenStack client for the authenticated user.
                var client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.Email,
                    user.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    organization.OpenstackProjectId
                );

                // Authenticate with OpenStack and get the token.
                var openstackToken = await client.Authenticate();

                // Create a new Project object in OpenStack and attempt to edit the folder.
                var openstackProject = new Project(openstackToken.identityUrl, _httpClient);

                string editOpenstackProjectResult = await openstackProject.EditProject(openstackToken.token, new ReourcesRequestsObjects.ProjectEditRequestOb
                {
                    id = folderDb.OpenstackProjectId,
                    name = folder.name,
                    description = folder.description
                });

                // If the OpenStack project edit fails, return "error".
                if (editOpenstackProjectResult == "error")
                    return "error";

                // Update the folder's name and description in the local database.
                folderDb.Name = folder.name.Trim();
                folderDb.DescriptionText = folder.description.Trim();

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "folder_edited" if the operation is successful.
                return "folder_edited";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "folders", "EditFolder", ex);
                return "error";
            }
        }

        /// <summary>
        /// Modifies the user links associated with a specific project (folder). 
        /// This includes adding new users to the project and removing existing users who are no longer linked.
        /// </summary>
        /// <param name="links">An object containing the project ID and the list of user IDs to be linked to the project.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the project ID is invalid or an exception occurs.
        /// - "links_modified" if the user links are successfully modified.
        /// </returns>
        public async Task<string> ModifyProjectLinks(ModifyFolderLinks links)
        {
            // Validate the project ID. Return "error" if the project ID is invalid.
            if (links.id == -1)
                return "error";

            try
            {
                // Retrieve the current list of user IDs associated with the specified project.
                var currentUsersInProject = _context.ProjectsUsers
                    .Where(pu => pu.ProjectId == links.id)
                    .Select(pu => (int?)pu.UserId)
                    .ToList();

                // Convert the list of new user IDs from the input to nullable integers.
                var userIdsAsNullable = links.userIds.Select(id => (int?)id).ToList();

                // Determine which users need to be added by finding the difference between the desired and current user links.
                var usersToAdd = userIdsAsNullable.Except(currentUsersInProject).ToList();

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
                var usersToDelete = currentUsersInProject.Where(pu => !userIdsAsNullable.Contains(pu)).ToList();

                // Remove existing user-project links for each user that needs to be removed.
                foreach (var user in usersToDelete)
                {
                    _context.ProjectsUsers.Remove(_context.ProjectsUsers.FirstOrDefault(pu => pu.ProjectId == links.id && pu.UserId == user));
                }

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "links_modified" if the operation is successful.
                return "links_modified";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "folders", "ModifyProjectLinks", ex);
                return "error";
            }
        }

        /// <summary>
        /// Adds a new user to the system and associates the user with a specified folder (project).
        /// The method first creates the user in the system and then links the user to the given folder.
        /// </summary>
        /// <param name="user">An object containing the details of the user to be added.</param>
        /// <param name="folderId">The ID of the folder to which the user should be linked. Defaults to -1.</param>
        /// <param name="uid">The ID of the user performing the operation. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the folder ID or user ID is invalid, or if an exception occurs.
        /// - The result from the user creation operation if the user creation fails.
        /// - "user_created" if the user is successfully created and linked to the folder.
        /// </returns>
        public async Task<string> AddUser(addUserOb user, int folderId = -1, int uid = -1)
        {
            // Validate the folder ID and user ID. Return "error" if either is invalid.
            if (folderId == -1 || uid == -1)
                return "error";

            try
            {
                // Retrieve the user performing the operation from the database using the provided user ID.
                var userDb = _context.Users.FirstOrDefault(u => u.Id == uid);

                // Attempt to add the new user by calling the AddUser method from the _users service.
                var addUserResult = await _users.AddUser(user, uid);

                // If the user creation fails, return the result of the user creation operation.
                if (addUserResult.result != "user_created")
                    return addUserResult.result;

                // If the user creation is successful, associate the user with the specified folder.
                _context.ProjectsUsers.Add(new Models.ProjectsUser
                {
                    UserId = (long)addUserResult.id,
                    ProjectId = (long)folderId
                });

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "user_created" if the operation is successful.
                return "user_created";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "folders", "AddUser", ex);
                return "error";
            }
        }

        /// <summary>
        /// Changes the parent folder (project) of a specified folder in the system.
        /// The method updates the parent folder ID of the given folder to a new parent folder ID.
        /// </summary>
        /// <param name="folderId">The ID of the folder whose parent is to be changed. Defaults to -1.</param>
        /// <param name="parentId">The new parent folder ID to be set. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the folder ID is invalid, the parent ID is invalid, or an exception occurs.
        /// - "parent_changed" if the parent folder is successfully updated.
        /// </returns>
        public async Task<string> ChangeParent(int folderId = -1, int parentId = -1)
        {
            // Validate the folder ID and parent ID. Return "error" if the folder ID is invalid or the parent ID is invalid.
            if (folderId == -1 || parentId == -1)
                return "error";

            try
            {
                // Retrieve the folder from the database using the provided folder ID.
                var folder = _context.Projects.FirstOrDefault(p => p.Id == folderId);

                // If the folder is not found, return "error".
                if (folder == null)
                    return "error";

                // Update the parent ID of the folder.
                folder.ParentId = parentId;

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "parent_changed" if the operation is successful.
                return "parent_changed";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "folders", "ChangeParent", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes a specified folder (project) from both the local database and the OpenStack system.
        /// The method ensures that the folder has no sub-resources before deletion.
        /// </summary>
        /// <param name="folderId">The ID of the folder to be deleted. Defaults to -1.</param>
        /// <param name="uid">The ID of the user performing the operation. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the folder ID or user ID is invalid, the user or folder is not found, or an exception occurs.
        /// - "cannot_delete" if the folder has sub-resources and cannot be deleted.
        /// - "folder_deleted" if the folder is successfully deleted.
        /// </returns>
        public async Task<string> DeleteFolder(int folderId = -1, int uid = -1)
        {
            // Validate the folder ID and user ID. Return "error" if either is invalid.
            if (folderId == -1 || uid == -1)
                return "error";

            try
            {
                // Retrieve the user performing the operation from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);

                // If the user is not found, return "error".
                if (user == null)
                    return "error";

                // Retrieve the folder to be deleted from the database using the provided folder ID.
                var folder = _context.Projects.FirstOrDefault(p => p.Id == folderId);

                // If the folder is not found, return "error".
                if (folder == null)
                    return "error";

                // Check if the folder has any sub-resources (child projects).
                var checkIfSubResources = _context.Projects.FirstOrDefault(p => p.ParentId == folderId);

                // If sub-resources exist, the folder cannot be deleted. Return "cannot_delete".
                if (checkIfSubResources != null)
                    return "cannot_delete";

                // Create an OpenStack client for the authenticated user.
                var client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.Email,
                    user.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    folder.OpenstackProjectId
                );

                // Authenticate with OpenStack and obtain a token.
                var openstackToken = await client.Authenticate();

                // Create a new Project object in OpenStack and attempt to delete the folder.
                var openstackProject = new Project(openstackToken.identityUrl, _httpClient);

                // Attempt to delete the project in OpenStack.
                var deleteOpenstackProjectResult = await openstackProject.DeleteProject(openstackToken.token, folder.OpenstackProjectId);

                // If the deletion in OpenStack fails, return "error".
                if (deleteOpenstackProjectResult != null && deleteOpenstackProjectResult == "error")
                    return "error";

                // Remove all user associations with the folder in the local database.
                _context.ProjectsUsers.RemoveRange(_context.ProjectsUsers.Where(pu => pu.ProjectId == folderId));

                // Remove the folder from the local database.
                _context.Projects.Remove(folder);

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "folder_deleted" if the operation is successful.
                return "folder_deleted";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "folders", "DeleteFolder", ex);
                return "error";
            }
        }
    }

    public class folderDetailsOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? openstackId { get; set; }
        public string? description { get; set; }
        public folderParentDetailOb? parent { get; set; }
    }

    public class folderParentDetailOb
    {
        public int? id { get; set; } 
        public string? name { get; set; }
        public string? scope { get; set; }
    }

    public class folderLinksOb
    {
        public int? userId { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public bool? isInProject { get; set; }
    }

    public class AddFolderOb
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public int? parentId { get; set; }
    }

    public class EditFolderOb
    {
        public int? id { get; set; } = -1;
        public string? name { get; set; }
        public string? oldName { get; set; }
        public string? description { get; set; }
    }

    public class ModifyFolderLinks
    {
        public int? id { get; set; } = -1;
        public List<int>? userIds { get; set; }
    }
}
