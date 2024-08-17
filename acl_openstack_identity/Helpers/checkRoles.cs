using acl_openstack_identity.Data;
using acl_openstack_identity.libraries;
using Newtonsoft.Json;
using Npgsql;
using OpenSearch.Client;

namespace acl_openstack_identity.Helpers
{
    public class checkRoles
    {
        private readonly OpenstackContext _context;

        public checkRoles(OpenstackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks whether a user has a specific permission within a project.
        /// The method retrieves the user's roles in the specified project and its parent projects, 
        /// then checks if the user is permitted to perform a specific action based on the role's permissions.
        /// </summary>
        /// <param name="main">The main category of the permission to check (e.g., resource type).</param>
        /// <param name="type">The type or sub-category of the permission to check.</param>
        /// <param name="perm">The specific permission to check (e.g., read, write).</param>
        /// <param name="uid">The ID of the user for whom the permission is being checked. Defaults to -1.</param>
        /// <param name="projectId">The ID of the project in which the permission is being checked. Defaults to -1.</param>
        /// <returns>
        /// A boolean value indicating whether the user has the specified permission:
        /// - Returns false if the user ID or project ID is invalid, if the project or roles are not found, 
        ///   or if the user does not have the specified permission.
        /// - Returns true if the user has the specified permission.
        /// </returns>
        public async Task<bool> isPermitted(string main, string type, string perm, int uid = -1, int projectId = -1)
        {
            // Validate the user ID and project ID. Return false if either is invalid.
            if (uid == -1 || projectId == -1)
                return false;

            try
            {
                // Initialize a list to store the roles associated with the user in the specified project and its parents.
                List<Models.Role> roles = new List<Models.Role>();

                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return false.
                if (project == null)
                    return false;

                // Recursive function to retrieve the roles of the user in the specified project and its parent projects.
                async Task GetRoleId(Models.Project projectDb)
                {
                    if (projectDb == null)
                        return;

                    // Retrieve the roles of the user in the current project.
                    var role = _context.Roles
                        .Where(r => r.UsersRoles.Any(ur => ur.UserId == uid && ur.RoleId == r.Id) && r.ProjectId == projectDb.Id)
                        .ToList();

                    // If roles are found, add them to the roles list.
                    if (role.Count != 0)
                    {
                        roles.AddRange(role);
                    }
                    else
                    {
                        // If no roles are found in the current project, check the parent project recursively.
                        await GetRoleId(_context.Projects.FirstOrDefault(p => p.Id == projectDb.ParentId));
                    }
                }

                // Start the recursive role retrieval from the specified project.
                await GetRoleId(project);

                // If no roles are found, return false.
                if (roles.Count == 0)
                    return false;

                // Retrieve the permission structure for the project's scope.
                var permissionStructure = rolesLibrary.GetRolesStructure(scopeSelector.GetScopeString((int)project.Scope));

                // Iterate through each role and its associated permissions.
                foreach (var role in roles)
                {
                    // Deserialize the permissions from JSON.
                    var permissions = JsonConvert.DeserializeObject<List<string>>(role.Permissions);

                    if (permissions == null) continue;

                    // Iterate through each permission and update the permission structure accordingly.
                    foreach (var permission in permissions)
                    {
                        string[] parts = permission.Split('_');

                        if (parts.Length == 3 && permissionStructure.ContainsKey(parts[0]))
                        {
                            if (permissionStructure[parts[0]].ContainsKey(parts[1]))
                            {
                                permissionStructure[parts[0]][parts[1]][parts[2]] = true;
                            }
                        }
                    }
                }

                // Check if the specified permission exists in the updated permission structure.
                return permissionStructure.ContainsKey(main) &&
                    permissionStructure[main].ContainsKey(type) &&
                    permissionStructure[main][type].ContainsKey(perm) &&
                    permissionStructure[main][type][perm];
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return false.
                Logger.SendException("Openstack_Panel", "checkRoles", "isPermitted", ex);
                return false;
            }
        }

        /// <summary>
        /// Retrieves the sidebar permissions for a user within a specific project.
        /// The method checks the user's roles and their associated permissions to determine what items in the sidebar should be accessible to the user.
        /// </summary>
        /// <param name="projectId">The ID of the project for which sidebar permissions are being retrieved. Defaults to -1.</param>
        /// <param name="uid">The ID of the user for whom the sidebar permissions are being retrieved. Defaults to -1.</param>
        /// <returns>
        /// An object representing the sidebar permissions, with boolean values indicating access to different parts of the sidebar.
        /// Returns false if the user ID or project ID is invalid, if no roles are found, or if an exception occurs during database access.
        /// </returns>
        public async Task<Object> GetSidebarPerms(int projectId = -1, int uid = -1)
        {
            // Validate the user ID and project ID. Return false if either is invalid.
            if (uid == -1 || projectId == -1)
                return false;

            try
            {
                // Initialize a list to store the roles associated with the user in the specified project and its parents.
                List<Models.Role> roles = new List<Models.Role>();

                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return false.
                if (project == null)
                    return false;

                // Recursive function to retrieve the roles of the user in the specified project and its parent projects.
                async Task GetRoleId(Models.Project projectDb)
                {
                    if (projectDb == null)
                        return;

                    // Retrieve the roles of the user in the current project.
                    var role = _context.Roles
                        .Where(r => r.UsersRoles.Any(ur => ur.UserId == uid && ur.RoleId == r.Id) && r.ProjectId == projectDb.Id)
                        .ToList();

                    // If roles are found, add them to the roles list.
                    if (role.Count != 0)
                    {
                        roles.AddRange(role);
                    }
                    else
                    {
                        // If no roles are found in the current project, check the parent project recursively.
                        await GetRoleId(_context.Projects.FirstOrDefault(p => p.Id == projectDb.ParentId));
                    }
                }

                // Start the recursive role retrieval from the specified project.
                await GetRoleId(project);

                // If no roles are found, return false.
                if (roles.Count == 0)
                    return false;

                // Retrieve the permission structure for the project's scope.
                var permissionStructure = rolesLibrary.GetRolesStructure(scopeSelector.GetScopeString((int)project.Scope));

                // Iterate through each role and its associated permissions.
                foreach (var role in roles)
                {
                    // Deserialize the permissions from JSON.
                    var permissions = JsonConvert.DeserializeObject<List<string>>(role.Permissions);

                    if (permissions == null) continue;

                    // Iterate through each permission and update the permission structure accordingly.
                    foreach (var permission in permissions)
                    {
                        string[] parts = permission.Split('_');

                        if (parts.Length == 3 && permissionStructure.ContainsKey(parts[0]))
                        {
                            if (permissionStructure[parts[0]].ContainsKey(parts[1]))
                            {
                                permissionStructure[parts[0]][parts[1]][parts[2]] = true;
                            }
                        }
                    }
                }

                // Get the sidebar construction object based on the project's scope.
                var sidebarPermOb = rolesLibrary.GetSidebarConstruct(scopeSelector.GetScopeString((int)project.Scope));

                // Update the sidebar permission object based on the permission structure.
                foreach (var mainKey in permissionStructure.Keys)
                {
                    foreach (var subKey in permissionStructure[mainKey].Keys)
                    {
                        if (permissionStructure[mainKey][subKey].ContainsKey("list") && permissionStructure[mainKey][subKey]["list"])
                        {
                            string propName = $"{mainKey}_{subKey}";

                            var prop = sidebarPermOb.GetType().GetProperty(propName);
                            if (prop != null && prop.PropertyType == typeof(bool))
                                prop.SetValue(sidebarPermOb, true);
                        }
                    }
                }

                // Return the updated sidebar permissions object.
                return sidebarPermOb;
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return false.
                Logger.SendException("Openstack_Panel", "checkRoles", "GetSidebarPerms", ex);
                return false;
            }
        }
    }
}
