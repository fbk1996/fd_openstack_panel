using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using Npgsql;
using acl_openstack_identity.Models;
using Microsoft.EntityFrameworkCore;
using acl_openstack_identity.libraries;
using Newtonsoft.Json;

namespace acl_openstack_identity.features
{
    public class roles
    {
        private readonly OpenstackContext _context;

        public roles (OpenstackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of roles based on the specified scope and project ID.
        /// </summary>
        /// <param name="scope">The scope as a string to filter the roles.</param>
        /// <param name="projectId">The ID of the project to filter the roles. Defaults to -1.</param>
        /// <returns>A list of rolesOb objects matching the criteria. If the criteria are not met or an exception occurs, an empty list is returned.</returns>
        public List<rolesOb> GetRoles (string scope, int projectId = -1)
        {
            // Check if the scope is null or empty, or if projectId is -1. If so, return an empty list.
            if (string.IsNullOrEmpty(scope) || projectId == -1)
                return new List<rolesOb>();

            try
            {
                // Start with a queryable collection of roles from the context.
                IQueryable<Role> query = _context.Roles;
                // Filter the roles by scope if scope is not null or empty.
                if (!string.IsNullOrEmpty(scope))
                    query = query.Where(r => r.Scope == scopeSelector.GetScopeInt(scope));
                // Filter the roles by projectId if projectId is not -1.
                if (projectId != -1)
                    query = query.Where(r => r.ProjectId == projectId);
                // Project the roles into a list of rolesOb, with membersCount as the count of the filtered roles.
                var roles = query
                    .AsNoTracking()
                    .Select(r => new rolesOb
                    {
                        id = (int)r.Id,
                        name = r.Name,
                        scope = scope,
                        membersCount = _context.UsersRoles.Count(r => r.RoleId == r.Id)
                    }).ToList();
                // Return the list of rolesOb objects.
                return roles;
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return an empty list if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "GetRoles", ex);
                return new List<rolesOb>();
            }
        }

        /// <summary>
        /// Retrieves detailed information about a role based on its ID.
        /// </summary>
        /// <param name="id">The ID of the role to retrieve details for. Defaults to -1.</param>
        /// <returns>
        /// A rolesDetailOb object containing the role's details, including its name, scope, and permissions structure.
        /// If the role is not found or an exception occurs, a rolesDetailOb with id = -1 is returned.
        /// </returns>
        public rolesDetailOb GetRoleDetails(int id = -1)
        {
            // Return a rolesDetailOb with id = -1 if the provided id is invalid.
            if (id == -1)
                return new rolesDetailOb { id = -1 };

            try
            {
                // Retrieve the role from the database without tracking changes in the context.
                var roleDb = _context.Roles
                    .AsNoTracking()
                    .FirstOrDefault(r => r.Id == id);

                // If the role is not found, return a rolesDetailOb with id = -1.
                if (roleDb == null)
                    return new rolesDetailOb { id = -1 };

                // Get the permission structure for the role's scope.
                var permissionStructure = rolesLibrary.GetRolesStructure(scopeSelector.GetScopeString((int)roleDb.Scope));

                // Deserialize the role's permissions from JSON to a list of strings.
                var permissions = JsonConvert.DeserializeObject<List<string>>(roleDb.Permissions);

                // Iterate over each permission and update the permission structure.
                foreach (var permission in permissions)
                {
                    string[] parts = permission.Split('_');

                    // Ensure the permission is correctly structured and update the permission structure.
                    if (parts.Length == 3 && permissionStructure.ContainsKey(parts[0]) && permissionStructure[parts[0]].ContainsKey(parts[1]))
                    {
                        permissionStructure[parts[0]][parts[1]][parts[2]] = true;
                    }
                }

                // Create a rolesDetailOb object with the role's details.
                var role = new rolesDetailOb
                {
                    id = id,
                    name = roleDb.Name,
                    scope = scopeSelector.GetScopeString((int)roleDb.Scope),
                    permissions = permissionStructure
                };

                // Return the role object. If null (unexpected), return a rolesDetailOb with id = -1.
                return role ?? new rolesDetailOb { id = -1 };
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return a rolesDetailOb with id = -1 if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "GetRoleDetails", ex);
                return new rolesDetailOb { id = -1 };
            }
        }

        /// <summary>
        /// Retrieves a list of links associated with a specified role ID.
        /// </summary>
        /// <param name="roleId">The ID of the role for which links are being retrieved. Defaults to -1.</param>
        /// <returns>
        /// A list of rolesLinksOb objects representing users linked to the role and its associated projects.
        /// Returns an empty list if the role ID is invalid or not found, or if an exception occurs.
        /// </returns>
        public List<rolesLinksOb> GetLinks(int roleId = -1)
        {
            // Return an empty list if the provided roleId is invalid.
            if (roleId == -1)
                return new List<rolesLinksOb>();

            try
            {
                // Retrieve the role from the database using the role ID.
                var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);

                // If the role is not found, return an empty list.
                if (role == null)
                    return new List<rolesLinksOb>();

                // Retrieve the project associated with the role.
                var project = _context.Projects.FirstOrDefault(p => p.Id == role.ProjectId);

                // If the project is not found, return an empty list.
                if (project == null)
                    return new List<rolesLinksOb>();

                // Create a list to hold the project IDs associated with the role.
                var projectIds = new List<int> { (int)project.Id };

                // Recursive function to add parent project IDs to the list.
                void AddParentObject(int projectId)
                {
                    if (projectId == null)
                        return;

                    // Retrieve the parent project based on the project ID.
                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                    // If the parent project exists, add its ID to the list and call the function recursively.
                    if (parentProject != null)
                    {
                        projectIds.Add((int)parentProject.Id);

                        AddParentObject((int)parentProject.ParentId);
                    }
                }

                // Add parent project IDs to the projectIds list.
                AddParentObject((int)project.ParentId);

                // Start with a queryable collection of users from the context.
                IQueryable<User> query = _context.Users;

                // Filter users based on their association with the retrieved project IDs.
                query = query.Where(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(up => up.UserId == u.Id).Select(up => up.ProjectId))));

                // Project the users into a list of rolesLinksOb objects, checking if each user is linked to the role.
                var links = query
                    .AsNoTracking() // Avoids tracking changes to the entities in the context.
                    .Select(l => new rolesLinksOb
                    {
                        id = (int)l.Id,
                        name = l.Name,
                        lastname = l.Lastname,
                        isAdded = l.UsersRoles.Any(ur => ur.RoleId == roleId && ur.UserId == l.Id) // Check if user is linked to the role.
                    }).ToList();

                // Return the list of rolesLinksOb objects.
                return links;
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return an empty list if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "GetLinks", ex);
                return new List<rolesLinksOb>();
            }
        }

        /// <summary>
        /// Adds an "Owner" role with specific permissions to the database for a given scope and project.
        /// </summary>
        /// <param name="scope">The scope as a string to define the role's scope.</param>
        /// <param name="projectId">The ID of the project to associate with the role. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation: "owner_role_added" if successful or "error" if the operation fails or inputs are invalid.
        /// </returns>
        public async Task<string> AddOwnerRole(string scope, int projectId = -1)
        {
            // Return "error" if the provided scope is null or empty.
            if (string.IsNullOrEmpty(scope))
                return "error";

            // Return "error" if the projectId is invalid.
            if (projectId == -1)
                return "error";

            try
            {
                // Retrieve the owner role structure based on the provided scope.
                var ownerRole = rolesLibrary.GetOwnerRole(scope);

                // Create a list to store permissions in the format "mainCatalog_permSection_perm".
                List<string> ownerList = new List<string>();

                // Iterate through the owner role's structure to construct the list of permissions.
                foreach (var mainCatalog in ownerRole)
                {
                    foreach (var permSection in mainCatalog.Value)
                    {
                        foreach (var perm in permSection.Value)
                        {
                            // Add the permission string to the owner list.
                            ownerList.Add($"{mainCatalog.Key}_{permSection.Key}_{perm.Key}");
                        }
                    }
                }

                // Add a new Role entity to the context with the name "Owner", associated projectId, scope, and serialized permissions.
                _context.Roles.Add(new Role
                {
                    Name = "Owner",
                    ProjectId = projectId,
                    Scope = (short)scopeSelector.GetScopeInt(scope), // Convert scope string to its integer representation.
                    Permissions = JsonConvert.SerializeObject(ownerList) // Serialize the list of permissions to JSON.
                });

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "owner_role_added" if the operation is successful.
                return "owner_role_added";
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "AddOwnerRole", ex);
                return "error";
            }
        }

        public async Task<string> AddDeleteRole(string scope, int _projectId = -1)
        {
            if (string.IsNullOrEmpty(scope))
                return "error";

            if (_projectId == -1)
                return "error";

            try
            {
                var deleteRole = rolesLibrary.GetRolesStructure(scope);

                List<string> deleteList = new List<string>();

                // Iterate through the owner role's structure to construct the list of permissions.
                foreach (var mainCatalog in deleteRole)
                {
                    foreach (var permSection in mainCatalog.Value)
                    {
                        foreach (var perm in permSection.Value)
                        {
                            // Add the permission string to the owner list.
                            deleteList.Add($"{mainCatalog.Key}_{permSection.Key}_{perm.Key}");
                        }
                    }
                }

                // Add a new Role entity to the context with the name "Owner", associated projectId, scope, and serialized permissions.
                _context.Roles.Add(new Role
                {
                    Name = "Owner",
                    ProjectId = _projectId,
                    Scope = (short)scopeSelector.GetScopeInt(scope), // Convert scope string to its integer representation.
                    Permissions = JsonConvert.SerializeObject(deleteList) // Serialize the list of permissions to JSON.
                });

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                return "delete_role_added";
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "roles", "AddDeleteRole", ex);
                return "error";
            }
        }

        /// <summary>
        /// Adds a new role to the database based on the provided role information.
        /// </summary>
        /// <param name="role">An object containing the details of the role to be added, including name, scope, project ID, and permissions.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "no_name" if the role name is missing.
        /// - "error" if the scope is invalid or the project ID is missing.
        /// - "exists" if a role with the same name and project ID already exists.
        /// - "role_added" if the role is successfully added.
        /// </returns>
        public async Task<string> AddRole(rolesAddOb role)
        {
            // Check if the role name is null or empty; return "no_name" if it is.
            if (string.IsNullOrEmpty(role.name))
                return "no_name";

            // Check if the scope is null or empty; return "error" if it is.
            if (string.IsNullOrEmpty(role.scope))
                return "error";

            // Check if the project ID is invalid; return "error" if it is.
            if (role.projectId == -1)
                return "error";

            try
            {
                // Check if a role with the same name (case-insensitive) and project ID already exists in the database.
                var checkRole = _context.Roles.FirstOrDefault(r => r.Name.ToLower() == role.name.ToLower().Trim() && r.ProjectId == role.projectId);

                // If such a role already exists, return "exists".
                if (checkRole != null)
                    return "exists";

                // Add the new Role entity to the context with the specified name, project ID, scope, and permissions.
                _context.Roles.Add(new Role
                {
                    Name = role.name.Trim(), // Trim whitespace from the name.
                    ProjectId = role.projectId,
                    Scope = (short)scopeSelector.GetScopeInt(role.scope), // Convert scope string to its integer representation.
                    Permissions = JsonConvert.SerializeObject(role.permissions) // Serialize the permissions to JSON.
                });

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "role_added" if the role is successfully added.
                return "role_added";
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "AddRole", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits an existing role in the database based on the provided role information.
        /// </summary>
        /// <param name="role">An object containing the details of the role to be edited, including ID, name, and permissions.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the role ID is invalid or the role is not found in the database.
        /// - "exists" if a role with the same name and project ID already exists (and it's a different role).
        /// - "role_edited" if the role is successfully edited.
        /// </returns>
        public async Task<string> EditRole(editRoleOb role)
        {
            // Check if the role ID is invalid; return "error" if it is.
            if (role.id == -1)
                return "error";

            try
            {
                // Retrieve the role from the database using the role ID.
                var roleDb = _context.Roles.FirstOrDefault(r => r.Id == role.id);

                // If the role is not found, return "error".
                if (roleDb == null)
                    return "error";

                // Check if a role with the same name and project ID already exists (case-insensitive).
                var checkRole = _context.Roles.FirstOrDefault(r => r.Name.ToLower() == role.name.ToLower().Trim() && r.ProjectId == roleDb.ProjectId);

                // If such a role exists and the new name differs from the old name, return "exists".
                if (checkRole != null && role.name != role.oldName)
                    return "exists";

                // Update the role's name and permissions.
                roleDb.Name = role.name.Trim(); // Trim whitespace from the name.
                roleDb.Permissions = JsonConvert.SerializeObject(role.permissions); // Serialize the permissions to JSON.

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "role_edited" if the role is successfully edited.
                return "role_edited";
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "EditRole", ex);
                return "error";
            }
        }

        /// <summary>
        /// Modifies the links between users and a specified role by adding or removing user-role associations.
        /// </summary>
        /// <param name="links">An object containing the role ID and a list of user IDs to be linked with the role.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the role ID is invalid or an exception occurs.
        /// - "links_modified" if the user-role links are successfully modified.
        /// </returns>
        public async Task<string> ModifyLinks(rolesAddLinksOb links)
        {
            // Check if the role ID is invalid; return "error" if it is.
            if (links.roleId == -1)
                return "error";

            try
            {
                // Retrieve a list of current users linked to the specified role from the database.
                var currentUsersInRole = _context.UsersRoles
                    .AsNoTracking() // Avoids tracking changes to the entities in the context.
                    .Where(ur => ur.RoleId == links.roleId) // Filter by the specified role ID.
                    .Select(ur => (int?)ur.UserId) // Select user IDs as nullable integers.
                    .ToList();

                // Convert the provided user IDs to a list of nullable integers.
                var userIdsAsNullable = links.links.Select(id => (int?)id).ToList();

                // Determine which users need to be added to the role by finding users not currently linked.
                var userToAdd = userIdsAsNullable.Except(currentUsersInRole).ToList();

                // Add new user-role associations for each user that needs to be added.
                foreach (var user in userToAdd)
                {
                    _context.UsersRoles.Add(new UsersRole
                    {
                        UserId = (long)user, // Cast user ID to long.
                        RoleId = (long)links.roleId // Cast role ID to long.
                    });
                }

                // Determine which users need to be removed from the role.
                var usersToRemove = currentUsersInRole.Where(ur => !userIdsAsNullable.Contains(ur)).ToList();

                // Remove existing user-role associations for each user that needs to be removed.
                foreach (var user in usersToRemove)
                {
                    _context.UsersRoles.Remove(_context.UsersRoles.FirstOrDefault(ur => ur.UserId == user && ur.RoleId == links.roleId));
                }

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "links_modified" if the operation is successful.
                return "links_modified";
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "ModifyLinks", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes a role and its associated user-role links from the database based on the given role ID.
        /// </summary>
        /// <param name="id">The ID of the role to be deleted. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the role ID is invalid or an exception occurs.
        /// - "role_deleted" if the role and its links are successfully deleted.
        /// </returns>
        public async Task<string> DeleteRole(int id = -1)
        {
            // Check if the role ID is invalid; return "error" if it is.
            if (id == -1)
                return "error";

            try
            {
                // Remove all user-role associations linked to the specified role ID.
                _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ur.RoleId == id));

                // Remove the role itself from the database by finding the role with the specified ID.
                var roleToRemove = _context.Roles.FirstOrDefault(r => r.Id == id);
                if (roleToRemove != null)
                {
                    _context.Roles.Remove(roleToRemove);
                }

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "role_deleted" if the operation is successful.
                return "role_deleted";
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "DeleteRole", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes multiple roles and their associated user-role links from the database based on the provided list of role IDs.
        /// </summary>
        /// <param name="ids">A list of role IDs to be deleted.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the list of IDs is empty or an exception occurs.
        /// - "roles_deleted" if the roles and their links are successfully deleted.
        /// </returns>
        public async Task<string> DeleteRoles(List<int> ids)
        {
            // Check if the list of IDs is empty; return "error" if it is.
            if (ids.Count == 0)
                return "error";

            try
            {
                // Remove all user-role associations linked to the specified role IDs.
                _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ids.Contains((int)ur.RoleId)));

                // Remove all roles from the database that match the specified role IDs.
                _context.Roles.RemoveRange(_context.Roles.Where(r => ids.Contains((int)r.Id)));

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "roles_deleted" if the operation is successful.
                return "roles_deleted";
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return "error" if an error occurs during database access.
                Logger.SendException("acl_openstack", "roles", "DeleteRoles", ex);
                return "error";
            }
        }

    }

    public class rolesOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? scope { get; set; }
        public int? membersCount { get; set; }
    }

    public class rolesDetailOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? scope { get; set; }
        public Dictionary<string, Dictionary<string, Dictionary<string, bool>>>? permissions { get; set; }
    }

    public class rolesLinksOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public bool? isAdded { get; set; }
    }

    public class rolesAddLinksOb
    {
        public int? roleId { get; set; }
        public List<int>? links { get; set; }
    }

    public class rolesAddOb
    {
        public string? name { get; set; }
        public string? scope { get; set; }
        public int? projectId { get; set; } = -1;
        public List<string>? permissions { get; set; }
    }

    public class editRoleOb
    {
        public int? id { get; set; } = -1;
        public string? name { get; set; }
        public string? oldName { get; set; }
        public List<string>? permissions { get; set; }
    }
}
