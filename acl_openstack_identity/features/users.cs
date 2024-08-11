using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;
using System.Text.Json;
using acl_openstack_identity.Resources;

namespace acl_openstack_identity.features
{
    public class users
    {
        private readonly OpenstackContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private roles _roles;

        public users (OpenstackContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
            _roles = new roles(context);
        }

        /// <summary>
        /// Retrieves a list of users associated with a specified project and its parent projects.
        /// </summary>
        /// <param name="projectId">The ID of the project for which users are to be retrieved. Defaults to -1.</param>
        /// <returns>
        /// A list of <see cref="userOb"/> objects representing the users associated with the project.
        /// Returns an empty list if the project ID is invalid, the project is not found, or an exception occurs.
        /// </returns>
        public List<userOb> GetUsers(int projectId = -1)
        {
            // Check if the project ID is invalid; return an empty list if it is.
            if (projectId == -1)
                return new List<userOb>();

            try
            {
                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return an empty list.
                if (project == null)
                    return new List<userOb>();

                // Initialize a list to hold the project IDs associated with the current project and its parents.
                List<int> projectIds = new List<int> { (int)project.Id };

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

                // Filter the users based on their association with the project IDs in the projectIds list.
                query = query.Where(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(p => p.UserId == u.Id).Select(p => p.ProjectId))));

                // Project the filtered users into a list of userOb objects.
                var users = query
                    .AsNoTracking() // Avoids tracking changes to the entities in the context.
                    .Select(u => new userOb
                    {
                        id = (int)u.Id,
                        email = u.Email,
                        name = u.Name,
                        lastname = u.Lastname,
                        from = _context.Projects
                            .Where(p => p.Id == Convert.ToInt32(u.ProjectsUsers.Select(pu => pu.ProjectId).FirstOrDefault()))
                            .Select(p => p.Name)
                            .FirstOrDefault() // Get the name of the project the user is from.
                    }).ToList();

                // Return the list of userOb objects.
                return users;
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return an empty list if an error occurs during database access.
                Logger.SendException("acl_openstack", "users", "GetUsers", ex);
                return new List<userOb>();
            }
        }

        /// <summary>
        /// Retrieves detailed information about a user based on the provided user ID and project ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose details are to be retrieved. Defaults to -1.</param>
        /// <param name="projectId">The ID of the project to check the user's association with. Defaults to -1.</param>
        /// <returns>
        /// A <see cref="userDetailOb"/> object containing the user's details, roles within the project, and status codes:
        /// - If either the userId or projectId is invalid, returns a userDetailOb with id = -1.
        /// - If the user is not associated with the project or its parents, returns a userDetailOb with id = -2.
        /// - If successful, returns a userDetailOb with the user's details and roles.
        /// </returns>
        public userDetailOb GetUserDetails(int userId = -1, int projectId = -1)
        {
            // Validate the input parameters. If either userId or projectId is invalid, return a userDetailOb with id = -1.
            if (userId == -1 || projectId == -1)
                return new userDetailOb { id = -1 };

            try
            {
                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return a userDetailOb with id = -1.
                if (project == null)
                    return new userDetailOb { id = -1 };

                // Initialize a list to hold the project IDs associated with the current project and its parents.
                List<int> projectIds = new List<int> { (int)project.Id };

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

                // Check if the user is associated with any of the projects in the projectIds list.
                var checkUser = _context.Users.FirstOrDefault(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(pu => pu.UserId == userId).Select(pu => pu.ProjectId))));

                // If the user is not associated with the project or its parents, return a userDetailOb with id = -2.
                if (checkUser == null)
                    return new userDetailOb { id = -2 };

                // Retrieve the user's details and roles within the specified project.
                var user = (userDetailOb)_context.Users
                    .AsNoTracking() // Avoids tracking changes to the entities in the context.
                    .Where(u => u.Id == userId)
                    .Select(u => new userDetailOb
                    {
                        id = (int)u.Id,
                        name = u.Name,
                        lastname = u.Lastname,
                        email = u.Email,
                        phone = u.Phone,
                        icon = u.Icon,
                        roles = _context.Roles
                            .Where(r => r.ProjectId == projectId)
                            .Select(r => new usersRoleOb
                            {
                                id = (int)r.Id,
                                name = r.Name,
                                isAdded = _context.UsersRoles.Any(ur => ur.RoleId == r.Id && ur.UserId == u.Id) // Check if the user has this role.
                            }).ToList()
                    }).FirstOrDefault();

                // Return the user details, or if null, return a userDetailOb with id = -1.
                return user ?? new userDetailOb { id = -1 };
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return a userDetailOb with id = -1 if an error occurs during database access.
                Logger.SendException("acl_openstack", "users", "GetUserDetails", ex);
                return new userDetailOb { id = -1 };
            }
        }

        /// <summary>
        /// Retrieves a list of project links for a specific user within a specified project and its parent projects.
        /// </summary>
        /// <param name="userId">The ID of the user whose project links are to be retrieved. Defaults to -1.</param>
        /// <param name="projectId">The ID of the project to check the user's association with. Defaults to -1.</param>
        /// <returns>
        /// A list of <see cref="userProjectLinksOb"/> objects representing the user's links to projects within the specified project and its parent projects.
        /// Returns an empty list if the userId or projectId is invalid, the project is not found, or an exception occurs.
        /// </returns>
        public List<userProjectLinksOb> GetUserProjectLinks(int userId = -1, int projectId = -1)
        {
            // Validate the input parameters. If either userId or projectId is invalid, return an empty list.
            if (userId == -1 || projectId == -1)
                return new List<userProjectLinksOb>();

            try
            {
                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return an empty list.
                if (project == null)
                    return new List<userProjectLinksOb>();

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

                // Check if the user is associated with any of the projects in the projectIds list.
                var checkUser = _context.Users.FirstOrDefault(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(up => up.UserId == u.Id))));

                // If the user is not associated with the project or its parents, return an empty list.
                if (checkUser == null)
                    return new List<userProjectLinksOb>();

                // Start with a queryable collection of projects from the context.
                IQueryable<Models.Project> query = _context.Projects;

                // Filter the projects to include only those that match the collected project IDs and have a scope of 2.
                query = query.Where(p => projectIds.Contains((int)p.Id) && p.Scope == 2);

                // Project the filtered projects into a list of userProjectLinksOb objects.
                var links = query
                    .AsNoTracking() // Avoids tracking changes to the entities in the context.
                    .Select(p => new userProjectLinksOb
                    {
                        id = (int)p.Id,
                        name = p.Name,
                        isAdded = _context.ProjectsUsers.Any(up => up.UserId == userId && up.ProjectId == p.Id) // Check if the user is linked to this project.
                    }).ToList();

                // Return the list of userProjectLinksOb objects.
                return links;
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return an empty list if an error occurs during database access.
                Logger.SendException("acl_openstack", "users", "GetUserProjectLinks", ex);
                return new List<userProjectLinksOb>();
            }
        }

        /// <summary>
        /// Retrieves a list of folder links for a specific user within a specified project and its parent projects.
        /// </summary>
        /// <param name="userId">The ID of the user whose folder links are to be retrieved. Must be a valid user ID.</param>
        /// <param name="projectId">The ID of the project to check the user's association with. Must be a valid project ID.</param>
        /// <returns>
        /// A list of <see cref="userFolderLinksOb"/> objects representing the user's links to folders within the specified project and its parent projects.
        /// Returns an empty list if the userId or projectId is invalid, the project is not found, or an exception occurs.
        /// </returns>
        public List<userFolderLinksOb> GetUserFolderLinks(int userId, int projectId)
        {
            // Validate the input parameters. If either userId or projectId is invalid, return an empty list.
            if (userId == -1 || projectId == -1)
                return new List<userFolderLinksOb>();

            try
            {
                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return an empty list.
                if (project == null)
                    return new List<userFolderLinksOb>();

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

                // Check if the user is associated with any of the projects in the projectIds list.
                var checkUser = _context.Users.FirstOrDefault(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(up => up.UserId == u.Id))));

                // If the user is not associated with the project or its parents, return an empty list.
                if (checkUser == null)
                    return new List<userFolderLinksOb>();

                // Start with a queryable collection of projects from the context.
                IQueryable<Models.Project> query = _context.Projects;

                // Filter the projects to include only those that match the collected project IDs and have a scope of 1 (indicating folders).
                query = query.Where(p => projectIds.Contains((int)p.Id) && p.Scope == 1);

                // Project the filtered projects into a list of userFolderLinksOb objects.
                var links = query
                    .AsNoTracking() // Avoids tracking changes to the entities in the context.
                    .Select(p => new userFolderLinksOb
                    {
                        id = (int)p.Id,
                        name = p.Name,
                        isAdded = _context.ProjectsUsers.Any(up => up.UserId == userId && up.ProjectId == p.Id) // Check if the user is linked to this project.
                    }).ToList();

                // Return the list of userFolderLinksOb objects.
                return links;
            }
            catch (NpgsqlException ex)
            {
                // Log the exception and return an empty list if an error occurs during database access.
                Logger.SendException("acl_openstack", "users", "GetUserProjectLinks", ex);
                return new List<userFolderLinksOb>();
            }
        }

        /// <summary>
        /// Adds a new user to the system, creating the user in both Keycloak and OpenStack, and linking them to a specified project with specific roles.
        /// </summary>
        /// <param name="user">An object containing the details of the user to be added, including name, lastname, email, projectId, and roles.</param>
        /// <param name="uId">The ID of the admin user performing the operation.</param>
        /// <returns>
        /// An <see cref="addUserResultOb"/> object indicating the result of the operation:
        /// - "no_name" if the user name is missing.
        /// - "no_lastname" if the user lastname is missing.
        /// - "no_email" if the user email is missing.
        /// - "no_roles" if the user roles are not provided.
        /// - "error" if an error occurs during the process.
        /// - "user_created" if the user is successfully created or linked to the project.
        /// </returns>
        public async Task<addUserResultOb> AddUser(addUserOb user, int uId)
        {
            // Validate required fields. Return appropriate error messages if any required field is missing.
            if (string.IsNullOrEmpty(user.name))
                return new addUserResultOb { result = "no_name" };
            if (string.IsNullOrEmpty(user.lastname))
                return new addUserResultOb { result = "no_lastname" };
            if (string.IsNullOrEmpty(user.email))
                return new addUserResultOb { result = "no_email" };
            if (user.projectId == -1)
                return new addUserResultOb { id = -1 };
            if (user.roles.Count == 0)
                return new addUserResultOb { result = "no_roles" };

            try
            {
                // Check if the user already exists in the system by email.
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.email);

                if (existingUser == null)
                {
                    // If the user does not exist, generate passwords for Keycloak and OpenStack.
                    var password = generators.generatePassword(20);
                    var openstackPassword = generators.generatePassword(20);

                    // Function to obtain Keycloak access token.
                    async Task<string> getKeycloakAccessToken()
                    {
                        var tokenEndpoint = $"https://auth.mechapp.cloud/auth/realms/master/protocol/openid-connect/token";

                        var requestBody = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", "admin-cli" },
                    { "username",  _configuration["KeyCloak:Username"]},
                    { "password", _configuration["KeyCloak:Password"] }
                };

                        HttpClient client = new HttpClient();

                        var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));

                        if (!response.IsSuccessStatusCode)
                            return "error";

                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                        return tokenData["access_token"];
                    }

                    // Get Keycloak access token.
                    var token = await getKeycloakAccessToken();

                    if (token == "error")
                        return new addUserResultOb { result = "error" };

                    // Function to create a new user in Keycloak.
                    async Task<string> keycloakCreateUser()
                    {
                        var createUserEndpoint = "https://auth.mechapp.cloud/auth/admin/realms/acl_openstack/users";

                        var userKeyCloak = new
                        {
                            username = user.email,
                            email = user.email,
                            firstName = user.name,
                            lastName = user.lastname,
                            enabled = true,
                            credentials = new[]
                            {
                        new
                        {
                            type = "password",
                            value = password,
                            temporary = false
                        }
                    }
                        };

                        HttpClient client = new HttpClient();

                        var jsonContent = new StringContent(JsonSerializer.Serialize(userKeyCloak), Encoding.UTF8, "application/json");
                        if (_httpClient.DefaultRequestHeaders.Authorization != null)
                            _httpClient.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                        var response = await client.PostAsync(createUserEndpoint, jsonContent);

                        if (!response.IsSuccessStatusCode)
                            return "error";
                        else
                            return "user_created";
                    }

                    // Create the user in Keycloak.
                    var createKeycloakUserStatus = await keycloakCreateUser();

                    if (createKeycloakUserStatus == "error")
                        return new addUserResultOb { result = "error" };

                    // Get the admin user performing the operation.
                    var adminUser = _context.Users.FirstOrDefault(u => u.Id == uId);

                    if (adminUser == null)
                        return new addUserResultOb { result = "error" };

                    // Retrieve the project based on the provided project ID.
                    var project = _context.Projects.FirstOrDefault(p => p.Id == user.projectId);

                    if (project == null)
                        return new addUserResultOb { result = "error" };

                    // Determine the organization ID by checking the project's parent hierarchy.
                    int organizationId = (int)project.Id;

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

                    CheckForOrganizationProject((int)project.ParentId, organizationId);

                    // Retrieve the organization based on the organization ID.
                    var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                    if (organization == null)
                        return new addUserResultOb { result = "error" };

                    // Authenticate with OpenStack using the admin user's credentials.
                    OpenstackClient _client = new OpenstackClient(
                        _configuration["OpenStack:AuthUrl"],
                        adminUser.Email,
                        adminUser.Openstackpassword,
                        _configuration["OpenStack:Domain"],
                        organization.OpenstackProjectId
                        );

                    var openstackToken = await _client.Authenticate();

                    // Create the user in OpenStack.
                    User userOpenstack = new User(openstackToken.identityUrl, _httpClient);

                    var addUserOpenstackResult = await userOpenstack.CreateUser(openstackToken.token, new ReourcesRequestsObjects.UserRequest.UserRequestAdd
                    {
                        defaultProjectId = organization.OpenstackProjectId,
                        domain_id = _configuration["OpenStack:Domain"],
                        enabled = true,
                        name = user.email.Trim(),
                        password = openstackPassword
                    });

                    if (addUserOpenstackResult.name == "error")
                        return new addUserResultOb { result = "error" };

                    // Assign the user to the organization group in OpenStack.
                    Group groupOpenstack = new Group(openstackToken.identityUrl, _httpClient);

                    var assignUserToGroupResult = await groupOpenstack.AddUserToGroup(openstackToken.token, organization.Groupid, addUserOpenstackResult.Id);

                    if (assignUserToGroupResult == "error")
                        return new addUserResultOb { result = "error" };

                    // Add the new user to the local database.
                    var newUserDb = _context.Users.Add(new Models.User
                    {
                        Name = user.name.Trim(),
                        Lastname = user.lastname.Trim(),
                        Email = user.email.Trim(),
                        Phone = user.phone.Trim(),
                        OpenstackId = addUserOpenstackResult.Id,
                        Openstackpassword = openstackPassword,
                        Isfirstlogin = true
                    });

                    await _context.SaveChangesAsync();

                    // Associate the user with the project in the local database.
                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        UserId = newUserDb.Entity.Id,
                        ProjectId = (long)user.projectId
                    });

                    // Assign roles to the user.
                    foreach (var role in user.roles)
                    {
                        _context.UsersRoles.Add(new Models.UsersRole
                        {
                            RoleId = role,
                            UserId = newUserDb.Entity.Id
                        });
                    }

                    // Generate and store a temporary token for the user's first login.
                    DateTime expiryDate = DateTime.Now.AddHours(72);

                    _context.UsersTokens.Add(new Models.UsersToken
                    {
                        UserId = newUserDb.Entity.Id,
                        Token = password,
                        Expire = expiryDate
                    });

                    await _context.SaveChangesAsync();

                    // Send an email to the user with their login details.
                    Sender.SendAddUserEmail("Nowe konto w portalu", user.name.Trim(), user.lastname.Trim(), organization.Name, password, user.email.Trim());

                    return new addUserResultOb { result = "user_created", id = (int)newUserDb.Entity.Id };
                }
                else
                {
                    // If the user already exists, retrieve the admin user and project details.
                    var adminUser = _context.Users.FirstOrDefault(u => u.Id == uId);

                    if (adminUser == null)
                        return new addUserResultOb { result = "error" };

                    var project = _context.Projects.FirstOrDefault(p => p.Id == user.projectId);

                    if (project == null)
                        return new addUserResultOb { result = "error" };

                    // Determine the organization ID by checking the project's parent hierarchy.
                    int organizationId = (int)project.Id;

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

                    CheckForOrganizationProject((int)project.ParentId, organizationId);

                    // Retrieve the organization based on the organization ID.
                    var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                    if (organization == null)
                        return new addUserResultOb { result = "error" };

                    // Authenticate with OpenStack using the admin user's credentials.
                    OpenstackClient _client = new OpenstackClient(
                        _configuration["OpenStack:AuthUrl"],
                        adminUser.Email,
                        adminUser.Openstackpassword,
                        _configuration["OpenStack:Domain"],
                        organization.OpenstackProjectId
                        );

                    var openstackToken = await _client.Authenticate();

                    // Assign the existing user to the organization group in OpenStack.
                    Group groupOpenstack = new Group(openstackToken.identityUrl, _httpClient);

                    var assignUserToGroupResult = await groupOpenstack.AddUserToGroup(openstackToken.token, organization.Groupid, existingUser.OpenstackId);

                    if (assignUserToGroupResult == "error")
                        return new addUserResultOb { result = "error" };

                    // Associate the existing user with the project in the local database.
                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        UserId = existingUser.Id,
                        ProjectId = (long)user.projectId
                    });

                    // Assign roles to the existing user.
                    foreach (var role in user.roles)
                    {
                        _context.UsersRoles.Add(new Models.UsersRole
                        {
                            RoleId = role,
                            UserId = existingUser.Id
                        });
                    }

                    await _context.SaveChangesAsync();

                    // Send an email notifying the user of their addition to the organization.
                    Sender.SendAddExistingUserEmail("Dodanie do organizacji", existingUser.Name, existingUser.Lastname, organization.Name, existingUser.Email);

                    return new addUserResultOb { result = "user_created", id = (int)existingUser.Id };
                }
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("acl_openstack", "users", "AddUser", ex);
                return new addUserResultOb { result = "error" };
            }
        }

        /// <summary>
        /// Edits an existing user's details in the system, updating both Keycloak and OpenStack, and modifying the user's roles and information.
        /// </summary>
        /// <param name="user">An object containing the updated details of the user, including name, lastname, email, and roles.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "no_name" if the user name is missing.
        /// - "no_lastname" if the user lastname is missing.
        /// - "no_email" if the user email is missing.
        /// - "error" if an error occurs during the process.
        /// - "exist" if the new email already exists in the system and is different from the old email.
        /// - "user_edited" if the user is successfully edited.
        /// </returns>
        public async Task<string> EditUser(editUserOb user)
        {
            // Validate required fields. Return appropriate error messages if any required field is missing.
            if (string.IsNullOrEmpty(user.name))
                return "no_name";
            if (string.IsNullOrEmpty(user.lastname))
                return "no_lastname";
            if (string.IsNullOrEmpty(user.email))
                return "no_email";
            if (string.IsNullOrEmpty(user.oldEmail))
                return "error";
            if (user.roles.Count == 0)
                return "no_roles";

            try
            {
                // Retrieve the user from the database using the provided user ID.
                var userDb = _context.Users.FirstOrDefault(u => u.Id == user.id);

                // If the user is not found, return "error".
                if (userDb == null)
                    return "error";

                // Check if the new email already exists in the system and is different from the old email.
                var checkUser = _context.Users.FirstOrDefault(u => u.Email == user.email.Trim());
                if (checkUser != null && user.email != user.oldEmail)
                    return "exist";

                // Function to obtain Keycloak access token.
                async Task<string> getKeycloakAccessToken()
                {
                    var tokenEndpoint = $"https://auth.mechapp.cloud/auth/realms/master/protocol/openid-connect/token";

                    var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "admin-cli" },
                { "username",  _configuration["KeyCloak:Username"]},
                { "password", _configuration["KeyCloak:Password"] }
            };

                    HttpClient client = new HttpClient();

                    var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));

                    if (!response.IsSuccessStatusCode)
                        return "error";

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    return tokenData["access_token"];
                }

                // Get Keycloak access token.
                var token = await getKeycloakAccessToken();

                if (token == "error")
                    return "error";

                // Function to retrieve the user's ID from Keycloak using their old email.
                async Task<string> GetUserId(string token)
                {
                    var usersEndpoint = $"https://auth.mechapp.cloud/auth/admin/realms/acl_openstack/users?email=${user.oldEmail}";

                    if (_httpClient.DefaultRequestHeaders.Authorization != null)
                        _httpClient.DefaultRequestHeaders.Clear();

                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await _httpClient.GetAsync(usersEndpoint);

                    if (!response.IsSuccessStatusCode)
                        return "error";

                    var users = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(await response.Content.ReadAsStringAsync());

                    if (users.Count == 0)
                        return "not_found";

                    return users[0]["id"].ToString();
                }

                // Get the Keycloak user ID using the old email.
                var keycloakUser = await GetUserId(token);

                if (keycloakUser == "error")
                    return "error";
                if (keycloakUser == "not_found")
                    return "not_found";

                // Function to update the user details in Keycloak.
                async Task<string> UpdateKeycloakUser(string token)
                {
                    var updateUserEndpoint = $"https://auth.mechapp.cloud/admin/realms/acl_openstack/users/{keycloakUser}";

                    var updatedUser = new
                    {
                        firstName = user.name.Trim(),
                        lastName = user.lastname.Trim(),
                        email = user.email.Trim(),
                    };

                    var jsonContent = new StringContent(JsonSerializer.Serialize(updatedUser), Encoding.UTF8, "application/json");

                    if (_httpClient.DefaultRequestHeaders.Authorization != null)
                        _httpClient.DefaultRequestHeaders.Clear();

                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await _httpClient.PutAsync(updateUserEndpoint, jsonContent);

                    if (!response.IsSuccessStatusCode)
                        return "error";
                    else
                        return "user_edited";
                }

                // Update the user in Keycloak.
                var editedKeycloakUser = await UpdateKeycloakUser(token);

                if (editedKeycloakUser == "error")
                    return "error";

                // Retrieve the project based on the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == user.projectId);

                if (project == null)
                    return "error";

                // Determine the organization ID by checking the project's parent hierarchy.
                int organizationId = (int)project.Id;

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

                CheckForOrganizationProject((int)project.ParentId, organizationId);

                // Retrieve the organization based on the organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                // Authenticate with OpenStack using the user's old email and existing OpenStack password.
                OpenstackClient _client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.oldEmail,
                    userDb.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    organization.OpenstackProjectId
                );

                var openstackToken = await _client.Authenticate();

                // Update the user's email in OpenStack.
                User openstackUser = new User(openstackToken.identityUrl, _httpClient);

                var editOpenstackUser = await openstackUser.EditUser(openstackToken.token, new ReourcesRequestsObjects.UserRequest.UserRequestEdit
                {
                    id = userDb.OpenstackId,
                    name = user.email.Trim()
                });

                if (editOpenstackUser == "error")
                    return "error";

                // Update the user's details in the local database.
                userDb.Name = user.name;
                userDb.Lastname = user.lastname;
                userDb.Email = user.email;
                userDb.Phone = user.phone;

                // Manage the user's roles by adding new ones and removing outdated ones.
                var currentRolesInUser = _context.UsersRoles
                    .Where(ur => ur.UserId == user.id)
                    .Select(ur => (int?)ur.RoleId)
                    .ToList();

                var rolesIdsAsNullable = user.roles.Select(id => (int?)id).ToList();

                var rolesToAdd = rolesIdsAsNullable.Except(currentRolesInUser).ToList();

                foreach (var role in rolesToAdd)
                {
                    _context.UsersRoles.Add(new Models.UsersRole
                    {
                        UserId = userDb.Id,
                        RoleId = (long)role
                    });
                }

                var rolesToRemove = currentRolesInUser.Where(ur => !rolesIdsAsNullable.Contains(ur)).ToList();

                foreach (var role in rolesToRemove)
                {
                    _context.UsersRoles.Remove(_context.UsersRoles.FirstOrDefault(ur => ur.Id == role && ur.UserId == user.id));
                }

                await _context.SaveChangesAsync();

                return "user_edited";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("acl_openstack", "users", "EditUser", ex);
                return "error";
            }
        }

        /// <summary>
        /// Modifies the links between a user and projects in the system. Adds or removes the user's associations with specific projects.
        /// </summary>
        /// <param name="links">An object containing the user ID and the list of project IDs to which the user should be linked.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the user is not found or an exception occurs.
        /// - "project_links_modified" if the user's project links are successfully modified.
        /// </returns>
        public async Task<string> ModifyProjectLinks(userModifyProjectLinksOb links)
        {
            try
            {
                // Retrieve the user from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == links.userId);

                // If the user is not found, return "error".
                if (user == null)
                    return "error";

                // Retrieve the current list of projects linked to the user that have a scope of 2.
                var currentProjectsInUser = _context.ProjectsUsers
                    .Where(p => p.UserId == links.userId && p.Project.Scope == 2)
                    .Select(p => (int?)p.ProjectId)
                    .ToList();

                // Convert the list of project IDs from the input to nullable integers.
                var projectIdsAsNullable = links.projectLinksIds.Select(id => (int?)id).ToList();

                // Determine which projects need to be added by finding the difference between the desired and current project links.
                var projectsToAdd = projectIdsAsNullable.Except(currentProjectsInUser).ToList();

                // Add new project-user links for each project that needs to be added.
                foreach (var project in projectsToAdd)
                {
                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        UserId = user.Id,
                        ProjectId = (long)project
                    });
                }

                // Determine which projects need to be removed by finding the difference between the current and desired project links.
                var projectsToDelete = currentProjectsInUser.Where(up => !projectIdsAsNullable.Contains(up)).ToList();

                // Remove existing project-user links for each project that needs to be removed.
                foreach (var project in projectsToDelete)
                {
                    _context.ProjectsUsers.Remove(_context.ProjectsUsers.FirstOrDefault(up => up.ProjectId == project && up.UserId == user.Id));
                }

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "project_links_modified" if the operation is successful.
                return "project_links_modified";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("acl_openstack", "users", "ModifyProjectLinks", ex);
                return "error";
            }
        }

        /// <summary>
        /// Modifies the links between a user and folders in the system. Adds or removes the user's associations with specific folders.
        /// </summary>
        /// <param name="links">An object containing the user ID and the list of folder IDs to which the user should be linked.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the user is not found or an exception occurs.
        /// - "folder_links_modified" if the user's folder links are successfully modified.
        /// </returns>
        public async Task<string> ModifyFolderLinks(userModifyFolderLinksOb links)
        {
            try
            {
                // Retrieve the user from the database using the provided user ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == links.userId);

                // If the user is not found, return "error".
                if (user == null)
                    return "error";

                // Retrieve the current list of folders (projects with scope 1) linked to the user.
                var currentFoldersInUser = _context.ProjectsUsers
                    .Where(up => up.UserId == user.Id && up.Project.Scope == 1)
                    .Select(up => (int?)up.ProjectId)
                    .ToList();

                // Convert the list of folder IDs from the input to nullable integers.
                var folderIdsAsNullable = links.folderLinksIds.Select(id => (int?)id).ToList();

                // Determine which folders need to be added by finding the difference between the desired and current folder links.
                var foldersToAdd = folderIdsAsNullable.Except(currentFoldersInUser).ToList();

                // Add new folder-user links for each folder that needs to be added.
                foreach (var folder in foldersToAdd)
                {
                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        UserId = user.Id,
                        ProjectId = (long)folder
                    });
                }

                // Determine which folders need to be removed by finding the difference between the current and desired folder links.
                var foldersToDelete = currentFoldersInUser.Where(up => !folderIdsAsNullable.Contains(up)).ToList();

                // Remove existing folder-user links for each folder that needs to be removed.
                foreach (var folder in foldersToDelete)
                {
                    _context.ProjectsUsers.Remove(_context.ProjectsUsers.FirstOrDefault(up => up.ProjectId == folder && up.UserId == user.Id));
                }

                // Save changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "folder_links_modified" if the operation is successful.
                return "folder_links_modified";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("acl_openstack", "users", "ModifyFolderLinks", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes a user from the system. If the user is associated with a project that has a scope of 0, they are completely removed from Keycloak and OpenStack.
        /// If the project scope is not 0, the user is assigned a "No-Access" role instead of being deleted.
        /// </summary>
        /// <param name="userId">The ID of the user to be deleted. Defaults to -1.</param>
        /// <param name="projectId">The ID of the project associated with the user. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the user or project is not found, or an exception occurs.
        /// - "user_deleted" if the user is successfully deleted or assigned the "No-Access" role.
        /// - "not_found" if the user cannot be found in Keycloak.
        /// </returns>
        public async Task<string> DeleteUser(int userId = -1, int projectId = -1)
        {
            // Validate the input parameters. Return "error" if either userId or projectId is invalid.
            if (userId == -1 || projectId == -1)
                return "error";

            try
            {
                // Retrieve the user from the database using the provided user ID and project ID.
                var user = _context.Users.FirstOrDefault(u => u.Id == userId && u.ProjectsUsers.Any(up => up.ProjectId == projectId && up.UserId == userId));

                // If the user is not found, return "error".
                if (user == null)
                    return "error";

                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return "error".
                if (project == null)
                    return "error";

                // If the project's scope is 0, proceed with complete user deletion.
                if (project.Scope == 0)
                {
                    // Function to obtain Keycloak access token.
                    async Task<string> getKeycloakAccessToken()
                    {
                        var tokenEndpoint = $"https://auth.mechapp.cloud/auth/realms/master/protocol/openid-connect/token";

                        var requestBody = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", "admin-cli" },
                    { "username",  _configuration["KeyCloak:Username"]},
                    { "password", _configuration["KeyCloak:Password"] }
                };

                        HttpClient client = new HttpClient();
                        var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));

                        if (!response.IsSuccessStatusCode)
                            return "error";

                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                        return tokenData["access_token"];
                    }

                    // Get Keycloak access token.
                    var token = await getKeycloakAccessToken();

                    if (token == "error")
                        return "error";

                    // Function to retrieve the Keycloak user ID based on the user's email.
                    async Task<string> GetUserId(string token)
                    {
                        var usersEndpoint = $"https://auth.mechapp.cloud/auth/admin/realms/acl_openstack/users?email=${user.Email}";

                        if (_httpClient.DefaultRequestHeaders.Authorization != null)
                            _httpClient.DefaultRequestHeaders.Clear();

                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                        var response = await _httpClient.GetAsync(usersEndpoint);

                        if (!response.IsSuccessStatusCode)
                            return "error";

                        var users = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(await response.Content.ReadAsStringAsync());

                        if (users.Count == 0)
                            return "not_found";

                        return users[0]["id"].ToString();
                    }

                    // Get the Keycloak user ID.
                    var keycloakUser = await GetUserId(token);

                    if (keycloakUser == "error" || keycloakUser == "not_found")
                        return keycloakUser;

                    // Function to delete the user from Keycloak.
                    async Task<string> DeleteKeycloakUser(string token)
                    {
                        var deleteUserEndpoint = $"https://auth.mechapp.cloud/auth/admin/realms/acl_openstack/users/{keycloakUser}";

                        if (_httpClient.DefaultRequestHeaders.Authorization != null)
                            _httpClient.DefaultRequestHeaders.Clear();

                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                        var response = await _httpClient.DeleteAsync(deleteUserEndpoint);

                        if (!response.IsSuccessStatusCode)
                            return "error";
                        else
                            return "user_deleted";
                    }

                    // Delete the user from Keycloak.
                    var deleteKeycloakUserResult = await DeleteKeycloakUser(token);

                    // Determine the organization ID by checking the project's parent hierarchy.
                    int organizationId = (int)project.Id;

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

                    CheckForOrganizationProject((int)project.ParentId, organizationId);

                    // Retrieve the organization based on the organization ID.
                    var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                    // Authenticate with OpenStack using the user's email and password.
                    OpenstackClient _client = new OpenstackClient(
                        _configuration["OpenStack:AuthUrl"],
                        user.Email,
                        user.Openstackpassword,
                        _configuration["OpenStack:Domain"],
                        organization.OpenstackProjectId
                    );

                    var openstackToken = await _client.Authenticate();

                    // Delete the user from OpenStack.
                    User openstackUser = new User(openstackToken.identityUrl, _httpClient);

                    var deleteOpenstackUser = await openstackUser.DeleteUser(openstackToken.token, user.OpenstackId);

                    if (deleteOpenstackUser == "error")
                        return "error";

                    // Remove all user-related records from the database.
                    _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ur.UserId == user.Id));
                    _context.ProjectsUsers.RemoveRange(_context.ProjectsUsers.Where(up => up.UserId == user.Id));
                    _context.UsersTokens.RemoveRange(_context.UsersTokens.Where(ut => ut.UserId == user.Id));
                    _context.Users.Remove(user);

                    await _context.SaveChangesAsync();

                    return "user_deleted";
                }
                else
                {
                    // If the project's scope is not 0, assign the user a "No-Access" role instead of deleting them.
                    var checkDeleteRole = _context.Roles.FirstOrDefault(r => r.Name == "No-Access" && r.ProjectId == project.Id);

                    if (checkDeleteRole == null)
                    {
                        var deleteRolesResult = await _roles.AddDeleteRole(scopeSelector.GetScopeString((int)project.Scope), (int)project.Id);
                        if (deleteRolesResult == "error")
                            return "error";
                    }

                    var deleteRole = _context.Roles.FirstOrDefault(r => r.Name == "No-Access" && r.ProjectId == project.Id);

                    if (deleteRole == null)
                        return "error";

                    // Assign the "No-Access" role to the user.
                    _context.UsersRoles.Add(new Models.UsersRole
                    {
                        UserId = user.Id,
                        RoleId = deleteRole.Id
                    });

                    await _context.SaveChangesAsync();

                    return "user_deleted";
                }
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("acl_openstack", "users", "DeleteUser", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes a list of users from the system. If the users are associated with a project that has a scope of 0, they are completely removed from Keycloak and OpenStack.
        /// If the project scope is not 0, the users are assigned a "No-Access" role instead of being deleted.
        /// </summary>
        /// <param name="userIds">A list of user IDs to be deleted.</param>
        /// <param name="projectId">The ID of the project associated with the users. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if no users are specified, if the project ID is invalid, or an exception occurs.
        /// - "users_deleted" if the users are successfully deleted or assigned the "No-Access" role.
        /// - "not_found" if a user cannot be found in Keycloak.
        /// </returns>
        public async Task<string> DeleteUsers(List<int> userIds, int projectId = -1)
        {
            // Validate the input parameters. Return "error" if no users are specified or if the projectId is invalid.
            if (userIds.Count == 0 || projectId == -1)
                return "error";

            try
            {
                // Iterate through each user ID in the provided list.
                foreach (var userId in userIds)
                {
                    // Retrieve the user from the database using the user ID and project ID.
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId && u.ProjectsUsers.Any(up => up.ProjectId == projectId && up.UserId == userId));

                    // If the user is not found, return "error".
                    if (user == null)
                        return "error";

                    // Retrieve the project from the database using the provided project ID.
                    var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                    // If the project is not found, return "error".
                    if (project == null)
                        return "error";

                    // If the project's scope is 0, proceed with complete user deletion.
                    if (project.Scope == 0)
                    {
                        // Function to obtain Keycloak access token.
                        async Task<string> getKeycloakAccessToken()
                        {
                            var tokenEndpoint = $"https://auth.mechapp.cloud/auth/realms/master/protocol/openid-connect/token";

                            var requestBody = new Dictionary<string, string>
                    {
                        { "grant_type", "password" },
                        { "client_id", "admin-cli" },
                        { "username",  _configuration["KeyCloak:Username"]},
                        { "password", _configuration["KeyCloak:Password"] }
                    };

                            HttpClient client = new HttpClient();
                            var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));

                            if (!response.IsSuccessStatusCode)
                                return "error";

                            var responseContent = await response.Content.ReadAsStringAsync();
                            var tokenData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                            return tokenData["access_token"];
                        }

                        // Get Keycloak access token.
                        var token = await getKeycloakAccessToken();

                        if (token == "error")
                            return "error";

                        // Function to retrieve the Keycloak user ID based on the user's email.
                        async Task<string> GetUserId(string token)
                        {
                            var usersEndpoint = $"https://auth.mechapp.cloud/auth/admin/realms/acl_openstack/users?email=${user.Email}";

                            if (_httpClient.DefaultRequestHeaders.Authorization != null)
                                _httpClient.DefaultRequestHeaders.Clear();

                            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                            var response = await _httpClient.GetAsync(usersEndpoint);

                            if (!response.IsSuccessStatusCode)
                                return "error";

                            var users = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(await response.Content.ReadAsStringAsync());

                            if (users.Count == 0)
                                return "not_found";

                            return users[0]["id"].ToString();
                        }

                        // Get the Keycloak user ID.
                        var keycloakUser = await GetUserId(token);

                        if (keycloakUser == "error" || keycloakUser == "not_found")
                            return keycloakUser;

                        // Function to delete the user from Keycloak.
                        async Task<string> DeleteKeycloakUser(string token)
                        {
                            var deleteUserEndpoint = $"https://auth.mechapp.cloud/auth/admin/realms/acl_openstack/users/{keycloakUser}";

                            if (_httpClient.DefaultRequestHeaders.Authorization != null)
                                _httpClient.DefaultRequestHeaders.Clear();
                            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                            var response = await _httpClient.DeleteAsync(deleteUserEndpoint);

                            if (!response.IsSuccessStatusCode)
                                return "error";
                            else
                                return "user_deleted";
                        }

                        // Delete the user from Keycloak.
                        var deleteKeycloakUserResult = await DeleteKeycloakUser(token);

                        // Determine the organization ID by checking the project's parent hierarchy.
                        int organizationId = (int)project.Id;

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

                        CheckForOrganizationProject((int)project.ParentId, organizationId);

                        // Retrieve the organization based on the organization ID.
                        var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                        // Authenticate with OpenStack using the user's email and password.
                        OpenstackClient _client = new OpenstackClient(
                            _configuration["OpenStack:AuthUrl"],
                            user.Email,
                            user.Openstackpassword,
                            _configuration["OpenStack:Domain"],
                            organization.OpenstackProjectId
                        );

                        var openstackToken = await _client.Authenticate();

                        // Delete the user from OpenStack.
                        User openstackUser = new User(openstackToken.identityUrl, _httpClient);

                        var deleteOpenstackUser = await openstackUser.DeleteUser(openstackToken.token, user.OpenstackId);

                        if (deleteOpenstackUser == "error")
                            return "error";

                        // Remove all user-related records from the database.
                        _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ur.UserId == user.Id));
                        _context.ProjectsUsers.RemoveRange(_context.ProjectsUsers.Where(up => up.UserId == user.Id));
                        _context.UsersTokens.RemoveRange(_context.UsersTokens.Where(ut => ut.UserId == user.Id));
                        _context.Users.Remove(user);

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // If the project's scope is not 0, assign the user a "No-Access" role instead of deleting them.
                        var checkDeleteRole = _context.Roles.FirstOrDefault(r => r.Name == "No-Access" && r.ProjectId == project.Id);

                        if (checkDeleteRole == null)
                        {
                            var deleteRolesResult = await _roles.AddDeleteRole(scopeSelector.GetScopeString((int)project.Scope), (int)project.Id);
                            if (deleteRolesResult == "error")
                                return "error";
                        }

                        var deleteRole = _context.Roles.FirstOrDefault(r => r.Name == "No-Access" && r.ProjectId == project.Id);

                        if (deleteRole == null)
                            return "error";

                        // Assign the "No-Access" role to the user.
                        _context.UsersRoles.Add(new Models.UsersRole
                        {
                            UserId = user.Id,
                            RoleId = deleteRole.Id
                        });

                        await _context.SaveChangesAsync();
                    }
                }

                return "users_deleted";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("acl_openstack", "users", "DeleteUsers", ex);
                return "error";
            }
        }
    }

    public class userOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? from { get; set; }
    }

    public class userDetailOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? icon { get; set; }
        public List<usersRoleOb>? roles { get; set; }
    }

    public class userProjectLinksOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public bool? isAdded { get; set; }
    }

    public class userFolderLinksOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public bool? isAdded { get; set; }
    }

    public class userModifyProjectLinksOb
    {
        public int? userId { get; set; }
        public List<int>? projectLinksIds { get; set; }
    }

    public class userModifyFolderLinksOb
    {
        public int? userId { get; set; }
        public List<int>? folderLinksIds { get; set; }
    }

    public class addUserOb
    {
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public int? projectId { get; set; }
        public List<int>? roles { get; set; }
    }

    public class editUserOb
    {
        public int? id { get; set; } = -1;
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? oldEmail { get; set; }
        public int? projectId { get; set; } 
        public List<int>? roles { get; set; }
    }

    public class addUserResultOb
    {
        public string? result { get; set; }
        public int? id { get; set; }
    }

    public class usersRoleOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public bool? isAdded { get; set; }
    }
}
