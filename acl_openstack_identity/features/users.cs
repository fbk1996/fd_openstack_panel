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
        private readonly assecoOpenstackContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private roles _roles;

        public users (assecoOpenstackContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
            _roles = new roles(context);
        }

        public List<userOb> GetUsers(int projectId = -1)
        {
            if (projectId == -1)
                return new List<userOb>();

            try
            {
                var project  = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                if (project == null)
                    return new List<userOb>();

                List<int> projectIds = new List<int> { (int)project.Id };

                void GetParentProjects (int parentId)
                {
                    if (parentId == null)
                        return;

                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == parentId);

                    if (parentProject != null)
                    {
                        projectIds.Add((int)parentProject.Id);

                        GetParentProjects((int)parentProject.ParentId);
                    }
                }

                GetParentProjects((int)project.ParentId);

                IQueryable<Models.User> query = _context.Users;

                query = query.Where(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(p => p.UserId == u.Id).Select(p => p.ProjectId))));

                var users = query
                    .AsNoTracking()
                    .Select(u => new userOb
                    {
                        id = (int)u.Id,
                        email = u.Email,
                        name = u.Name,
                        lastname = u.Lastname,
                        from = _context.Projects
                            .Where(p => p.Id == Convert.ToInt32(u.ProjectsUsers.Select(pu => pu.ProjectId).FirstOrDefault()))
                            .Select(p => p.Name)
                            .FirstOrDefault()
                    }).ToList();

                return users;
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "users", "GetUsers", ex);
                return new List<userOb>();
            }
        }
        
        public userDetailOb GetUserDetails(int userId = -1, int projectId = -1)
        {
            if (userId == -1 || projectId == -1)
                return new userDetailOb { id = -1 };

            try
            {
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                if (project == null)
                    return new userDetailOb { id = -1 };

                List<int> projectIds = new List<int> { (int)project.Id };

                void GetParentProjects(int parentId)
                {
                    if (parentId == null)
                        return;

                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == parentId);

                    if (parentProject != null)
                    {
                        projectIds.Add((int)parentProject.Id);

                        GetParentProjects((int)parentProject.ParentId);
                    }
                }

                GetParentProjects((int)project.ParentId);

                var checkUser = _context.Users.FirstOrDefault(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(pu => pu.UserId == userId).Select(pu => pu.ProjectId))));

                if (checkUser == null)
                    return new userDetailOb { id = -2 };

                var user = (userDetailOb)_context.Users
                    .AsNoTracking()
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
                                isAdded = _context.UsersRoles.Any(ur => ur.RoleId == r.Id && ur.UserId == u.Id)
                            }).ToList()
                    }).FirstOrDefault();

                return user ?? new userDetailOb { id = -1 };
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "users", "GetUserDetails", ex);
                return new userDetailOb { id = -1 };
            }
        }

        public List<userProjectLinksOb> GetUserProjectLinks(int userId = -1, int projectId = -1)
        {
            if (userId == -1 || projectId == -1)
                return new List<userProjectLinksOb>();

            try
            {
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                if (project == null)
                    return new List<userProjectLinksOb>();

                List<int> projectIds = new List<int>();

                void GetParentProjects(int parentId)
                {
                    if (parentId == null)
                        return;

                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == parentId);

                    if (parentProject != null)
                    {
                        projectIds.Add((int)parentProject.Id);

                        GetParentProjects((int)parentProject.ParentId);
                    }
                }

                GetParentProjects((int)project.ParentId);

                var checkUser = _context.Users.FirstOrDefault(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(up => up.UserId == u.Id))));

                if (checkUser == null)
                    return new List<userProjectLinksOb>();

                IQueryable<Models.Project> query = _context.Projects;

                query = query.Where(p => projectIds.Contains((int)p.Id) && p.Scope == 2);

                var links = query
                    .AsNoTracking()
                    .Select(p => new userProjectLinksOb
                    {
                        id = (int)p.Id,
                        name = p.Name,
                        isAdded = _context.ProjectsUsers.Any(up => up.UserId == userId && up.ProjectId == p.Id)
                    }).ToList();

                return links;
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "users", "GetUserPorjectLinks", ex);
                return new List<userProjectLinksOb>();
            }
        }

        public List<userFolderLinksOb> GetUserFolderLinks (int userId, int projectId)
        {
            if (userId == -1 || projectId == -1)
                return new List<userFolderLinksOb>();

            try
            {
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                if (project == null)
                    return new List<userFolderLinksOb>();

                List<int> projectIds = new List<int>();

                void GetParentProjects(int parentId)
                {
                    if (parentId == null)
                        return;

                    var parentProject = _context.Projects.FirstOrDefault(p => p.Id == parentId);

                    if (parentProject != null)
                    {
                        projectIds.Add((int)parentProject.Id);

                        GetParentProjects((int)parentProject.ParentId);
                    }
                }

                GetParentProjects((int)project.ParentId);

                var checkUser = _context.Users.FirstOrDefault(u => projectIds.Contains(Convert.ToInt32(u.ProjectsUsers.Where(up => up.UserId == u.Id))));

                if (checkUser == null)
                    return new List<userFolderLinksOb>();

                IQueryable<Models.Project> query = _context.Projects;

                query = query.Where(p => projectIds.Contains((int)p.Id) && p.Scope == 1);

                var links = query
                    .AsNoTracking()
                    .Select(p => new userFolderLinksOb
                    {
                        id = (int)p.Id,
                        name = p.Name,
                        isAdded = _context.ProjectsUsers.Any(up => up.UserId == userId && up.ProjectId == p.Id)
                    }).ToList();

                return links;
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "users", "GetUserPorjectLinks", ex);
                return new List<userFolderLinksOb>();
            }
        }

        public async Task<addUserResultOb> AddUser(addUserOb user, int uId)
        {
            if (string.IsNullOrEmpty(user.name))
                return new addUserResultOb { result = "no_name" };
            if (string.IsNullOrEmpty(user.lastname))
                return new addUserResultOb { result = "no_lastname"};
            if (string.IsNullOrEmpty(user.email))
                return new addUserResultOb { result = "no_email" };

            if (user.projectId == -1)
                return new addUserResultOb { id = -1 };
            if (user.roles.Count == 0)
                return new addUserResultOb { result = "no_roles" };


            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.email);

                if (existingUser == null)
                {
                    var password = generators.generatePassword(20);
                    var openstackPassword = generators.generatePassword(20);

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

                    var token = await getKeycloakAccessToken();

                    if (token == "error")
                        return new addUserResultOb { result = "error" };

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

                    var createKeycloakUserStatus = await keycloakCreateUser();

                    if (createKeycloakUserStatus == "error")
                        return new addUserResultOb { result = "error" };

                    var adminUser = _context.Users.FirstOrDefault(u => u.Id == uId);

                    if (adminUser == null)
                        return new addUserResultOb { result = "error" };

                    var project = _context.Projects.FirstOrDefault(p => p.Id == user.projectId);

                    if (project == null)
                        return new addUserResultOb { result = "error" };

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

                    var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                    if (organization == null)
                        return new addUserResultOb { result = "error" };

                    OpenstackClient _client = new OpenstackClient(
                        _configuration["OpenStack:AuthUrl"],
                        adminUser.Email,
                        adminUser.Openstackpassword,
                        _configuration["OpenStack:Domain"],
                        organization.OpenstackProjectId
                        );

                    var openstackToken = await _client.Authenticate();

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

                    Group groupOpenstack = new Group(openstackToken.identityUrl, _httpClient);

                    var assignUserToGroupResult = await groupOpenstack.AddUserToGroup(openstackToken.token, organization.Groupid, addUserOpenstackResult.Id);

                    if (assignUserToGroupResult == "error")
                        return new addUserResultOb { result = "error" };

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

                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        UserId = newUserDb.Entity.Id,
                        ProjectId = (long)user.projectId
                    });

                    foreach (var role in user.roles)
                    {
                        _context.UsersRoles.Add(new Models.UsersRole
                        {
                            RoleId = role,
                            UserId = newUserDb.Entity.Id
                        });
                    }

                    DateTime expiryDate = DateTime.Now.AddHours(72);

                    _context.UsersTokens.Add(new Models.UsersToken
                    {
                        UserId = newUserDb.Entity.Id,
                        Token = password,
                        Expire = expiryDate
                    });

                    await _context.SaveChangesAsync();

                    Sender.SendAddUserEmail("Nowe konto w portalu", user.name.Trim(), user.lastname.Trim(), organization.Name, password, user.email.Trim());

                    return new addUserResultOb { result = "user_created", id = (int)newUserDb.Entity.Id };
                }
                else
                {
                    var adminUser = _context.Users.FirstOrDefault(u => u.Id == uId);

                    if (adminUser == null)
                        return new addUserResultOb { result = "error" };

                    var project = _context.Projects.FirstOrDefault(p => p.Id == user.projectId);

                    if (project == null)
                        return new addUserResultOb { result = "error" };

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

                    var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                    if (organization == null)
                        return new addUserResultOb { result = "error" };

                    OpenstackClient _client = new OpenstackClient(
                        _configuration["OpenStack:AuthUrl"],
                        adminUser.Email,
                        adminUser.Openstackpassword,
                        _configuration["OpenStack:Domain"],
                        organization.OpenstackProjectId
                        );

                    var openstackToken = await _client.Authenticate();

                    Group groupOpenstack = new Group(openstackToken.identityUrl, _httpClient);

                    var assignUserToGroupResult = await groupOpenstack.AddUserToGroup(openstackToken.token, organization.Groupid, existingUser.OpenstackId);

                    if (assignUserToGroupResult == "error")
                        return new addUserResultOb { result = "error" };

                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        UserId = existingUser.Id,
                        ProjectId = (long)user.projectId
                    });

                    foreach (var role in user.roles)
                    {
                        _context.UsersRoles.Add(new Models.UsersRole
                        {
                            RoleId = role,
                            UserId = existingUser.Id
                        });
                    }

                    await _context.SaveChangesAsync();

                    Sender.SendAddExistingUserEmail("Dodanie do organizacji", existingUser.Name, existingUser.Lastname, organization.Name, existingUser.Email);

                    return new addUserResultOb { result = "user_created", id = (int)existingUser.Id };
                }
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "users", "AddUser", ex);
                return new addUserResultOb { result = "error" };
            }
        }

        public async Task<string> EditUser(editUserOb user)
        {
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
                var userDb = _context.Users.FirstOrDefault(u => u.Id == user.id);

                if (userDb == null)
                    return "error";

                var checkUser = _context.Users.FirstOrDefault(u => u.Email == user.email.Trim());

                if (checkUser != null && user.email != user.oldEmail)
                    return "exist";

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

                var token = await getKeycloakAccessToken();

                if (token == "error")
                    return "error";

                async Task<string> GetUserId (string token)
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

                var keycloakUser = await GetUserId(token);

                if (keycloakUser == "error")
                    return "error";
                if (keycloakUser == "not_found")
                    return "not_found";

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

                var editedKeycloakUser = await UpdateKeycloakUser(token);

                if (editedKeycloakUser == "error")
                    return "error";

                var project = _context.Projects.FirstOrDefault(p => p.Id == user.projectId);

                if (project == null)
                    return "error" ;

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

                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                OpenstackClient _client = new OpenstackClient(
                        _configuration["OpenStack:AuthUrl"],
                        user.oldEmail,
                        userDb.Openstackpassword,
                        _configuration["OpenStack:Domain"],
                        organization.OpenstackProjectId
                        );

                var openstackToken = await _client.Authenticate();

                User openstackUser = new User(openstackToken.identityUrl, _httpClient);

                var editOpenstackUser = await openstackUser.EditUser(openstackToken.token, new ReourcesRequestsObjects.UserRequest.UserRequestEdit
                {
                    id = userDb.OpenstackId,
                    name = user.email.Trim()
                });

                if (editOpenstackUser == "error")
                    return "error";

                userDb.Name = user.name;
                userDb.Lastname = user.lastname;
                userDb.Email = user.email;
                userDb.Phone = user.phone;

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
                Logger.SendException("acl_openstack", "users", "EditUser", ex);
                return "error";
            }
        }

        public async Task<string> ModifyProjectLinks(userModifyProjectLinksOb links)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == links.userId);

                if (user == null)
                    return "error";

                var currentProjectsInUser = _context.ProjectsUsers
                    .Where(p => p.UserId == links.userId && p.Project.Scope == 2)
                    .Select(p => (int?)p.ProjectId)
                    .ToList();

                var projectIdsAsNullable = links.projectLinksIds.Select(id => (int?)id).ToList();

                var projectsToAdd = projectIdsAsNullable.Except(currentProjectsInUser).ToList();

                foreach(var project in projectsToAdd)
                {
                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        UserId = user.Id,
                        ProjectId = (long)project
                    });
                }

                var projectsToDelete = currentProjectsInUser.Where(up => !projectIdsAsNullable.Contains(up)).ToList();

                foreach (var project in projectsToDelete)
                {
                    _context.ProjectsUsers.Remove(_context.ProjectsUsers.FirstOrDefault(up => up.ProjectId == project && up.UserId == user.Id));
                }

                await _context.SaveChangesAsync();

                return "project_links_modified";
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "users", "ModifyProjectLinks", ex);
                return "error";
            }
        }

        public async Task<string> ModifyFolderLinks(userModifyFolderLinksOb links)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == links.userId);

                if (user == null)
                    return "error";

                var currentFoldersInUser = _context.ProjectsUsers
                    .Where(up => up.UserId == user.Id && up.Project.Scope == 1)
                    .Select(up => (int?)up.ProjectId)
                    .ToList();

                var folderIdsAsNullable = links.folderLinksIds.Select(id => (int?)id).ToList();

                var foldersToAdd = folderIdsAsNullable.Except(currentFoldersInUser).ToList();

                foreach (var folder in foldersToAdd)
                {
                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        UserId = user.Id,
                        ProjectId = (long)folder
                    });
                }

                var foldersToDelete = currentFoldersInUser.Where(up => !folderIdsAsNullable.Contains(up)).ToList();

                foreach (var folder in foldersToDelete)
                {
                    _context.ProjectsUsers.Remove(_context.ProjectsUsers.FirstOrDefault(up => up.ProjectId == folder && up.UserId == user.Id));
                }

                await _context.SaveChangesAsync();

                return "folder_links_modified";
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "users", "ModifyFolderLinks", ex);
                return "error";
            }
        }

        public async Task<string> DeleteUser(int userId = - 1, int projectId = -1)
        {
            if (userId == -1)
                return "error";
            if (projectId == -1)
                return "error";

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId && u.ProjectsUsers.Any(up => up.ProjectId == projectId && up.UserId == userId));

                if (user == null)
                    return "error";

                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                if (project == null)
                    return "error";

                if (project.Scope == 0)
                {
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

                    var token = await getKeycloakAccessToken();

                    if (token == "error")
                        return "error";

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

                    var keycloakUser = await GetUserId(token);

                    if (keycloakUser == "error")
                        return "error";
                    if (keycloakUser == "not_found")
                        return "not_found";

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

                    var deleteKeycloakUserResult = await DeleteKeycloakUser(token);

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

                    var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                    OpenstackClient _client = new OpenstackClient(
                            _configuration["OpenStack:AuthUrl"],
                            user.Email,
                            user.Openstackpassword,
                            _configuration["OpenStack:Domain"],
                            organization.OpenstackProjectId
                            );

                    var openstackToken = await _client.Authenticate();

                    User openstackUser = new User(openstackToken.identityUrl, _httpClient);

                    var deleteOpenstackUser = await openstackUser.DeleteUser(openstackToken.token, user.OpenstackId);

                    if (deleteOpenstackUser == "error")
                        return "error";

                    _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ur.UserId == user.Id));
                    _context.ProjectsUsers.RemoveRange(_context.ProjectsUsers.Where(up => up.UserId == user.Id));
                    _context.UsersTokens.RemoveRange(_context.UsersTokens.Where(ut => ut.UserId == user.Id));
                    _context.Users.Remove(user);

                    await _context.SaveChangesAsync();

                    return "user_deleted";
                }
                else
                {
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
                Logger.SendException("acl_openstack", "users", "DeleteUser", ex);
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
