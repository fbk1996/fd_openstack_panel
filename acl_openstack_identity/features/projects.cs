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

        public projectDetailsOb GetProjectDetails(int projectId = -1)
        {
            if (projectId == -1)
                return new projectDetailsOb { id = -1 };

            try
            {
                var project = (projectDetailsOb)_context.Projects
                    .AsNoTracking()
                    .Where(p => p.Id == projectId)
                    .Select(p => new projectDetailsOb
                    {
                        id = (int)p.Id,
                        parentProject = (p.ParentId != null) ? new projectLinksDetailOb
                        {
                            id = (int)p.Parent.Id,
                            name = p.Parent.Name
                        } : null,
                        name = p.Name,
                        openstackId = p.OpenstackProjectId
                    }).FirstOrDefault();

                return project ?? new projectDetailsOb { id = -1 };
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("openstack_panel", "projects", "GetProjectDetails", ex);
                return new projectDetailsOb { id = -1 };
            }
        }

        public List<projectLinksOb> GetProjectLinks(int projectId = -1)
        {
            if (projectId == -1)
                return null;

            try
            {
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

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

                IQueryable<Models.User> query = _context.Users;

                query = query.Where(u => u.ProjectsUsers.Any(up => projectIds.Contains((int)up.ProjectId) && up.Project.Scope == 2));

                var projectLinks = query
                    .AsNoTracking()
                    .Select(ul => new projectLinksOb
                    {
                        userId = (int)ul.Id,
                        name = (string)ul.Name,
                        lastname = (string)ul.Lastname,
                        isInProject = _context.ProjectsUsers.Any(up => up.ProjectId == projectId && up.UserId == ul.Id)
                    }).ToList();

                return projectLinks;
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("openstack_panel", "projects", "GetProjectLinks", ex);
                return null;
            }
        }

        public async Task<string> AddProject(AddProjectOb project, int uid = -1)
        {
            if (uid == -1)
                return "error";

            if (string.IsNullOrEmpty(project.name))
                return "no_name";

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);

                if (user == null)
                    return "error";

                var checkProject = _context.Projects.FirstOrDefault(p => p.Name.ToLower() == project.name.ToLower().Trim());

                if (checkProject != null)
                    return "exists";

                var projectDb = _context.Projects.FirstOrDefault(p => p.ProjectsUsers.Any(pu => pu.UserId == uid));

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

                CheckForOrganizationProject((int)projectDb.ParentId, organizationId);

                // Retrieve the organization based on the organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                if (organization == null)
                    return  "error" ;

                var parentProjectDb = _context.Projects.FirstOrDefault(p => p.Id == project.parentId);

                // Authenticate with OpenStack using the admin user's credentials.
                OpenstackClient _client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.Email,
                    user.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    organization.OpenstackProjectId
                    );

                var openstackToken = await _client.Authenticate();

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

                if (createProjectResult.Name == "error")
                    return "error";

                _context.Projects.Add(new Models.Project
                {
                    Name = project.name.Trim(),
                    OpenstackProjectId = createProjectResult.Id,
                    DescriptionText = project.description.Trim(),
                    Scope = 2,
                    CreationDate = DateTime.Now,
                });

                await _context.SaveChangesAsync();

                return "project_created";
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("openstack_panel", "projects", "AddProject", ex);
                return "error";
            }
        }

        public async Task<string> EditProject(EditProjectOb project, int? uid = -1)
        {
            if (uid == -1)
                return "error";

            if (string.IsNullOrEmpty(project.name) || string.IsNullOrEmpty(project.oldName))
                return "no_name";

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);

                if (user == null)
                    return "error";

                var checkIfExist = _context.Projects.FirstOrDefault(p => p.Name.ToLower() == project.name.ToLower().Trim());

                if (checkIfExist != null && project.name != project.oldName)
                    return "exists";

                

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

                CheckForOrganizationProject((int)projectDb.ParentId, organizationId);

                // Retrieve the organization based on the organization ID.
                var organization = _context.Projects.FirstOrDefault(p => p.Id == organizationId);

                if (organization == null)
                    return "error";

                // Authenticate with OpenStack using the admin user's credentials.
                OpenstackClient _client = new OpenstackClient(
                    _configuration["OpenStack:AuthUrl"],
                    user.Email,
                    user.Openstackpassword,
                    _configuration["OpenStack:Domain"],
                    organization.OpenstackProjectId
                    );

                var openstackToken = await _client.Authenticate();

                var openstackProject = new Project(openstackToken.identityUrl, _httpClient);

                var editProjectResult = await openstackProject.EditProject(openstackToken.token, new ReourcesRequestsObjects.ProjectEditRequestOb
                {
                    id = projectDb.OpenstackProjectId,
                    name = project.name,
                    description = project.description
                });

                if (editProjectResult != "error")
                    return "error";

                projectDb.Name = project.name.Trim();
                projectDb.DescriptionText = project.description.Trim();

                await _context.SaveChangesAsync();

                return "project_edited";
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("Openstack_panel", "projects", "EditProject", ex);
                return "error";
            }
        }

        public async Task<string> ModifyProjectLinks(ProjectModifyLinksOb links)
        {
            if (links.id == -1)
                return "error";

            try
            {
                var existingUsersInProjects = _context.ProjectsUsers
                    .Where(up => up.ProjectId == links.id)
                    .Select(up => (int?)up.UserId)
                    .ToList();

                var userIdsAsNullable = links.userIds.Select(id => (int?)id).ToList();

                var usersToAdd = userIdsAsNullable.Except(existingUsersInProjects).ToList();

                foreach (var user in usersToAdd)
                {
                    _context.ProjectsUsers.Add(new Models.ProjectsUser
                    {
                        ProjectId = (long)links.id,
                        UserId = (long)user
                    });
                }

                var usersToDelete = existingUsersInProjects.Where(up => !userIdsAsNullable.Contains((int?)up)).ToList();

                foreach (var user in usersToDelete)
                {
                    _context.ProjectsUsers.Remove(_context.ProjectsUsers.FirstOrDefault(up => up.UserId == user && up.ProjectId == links.id));
                }

                await _context.SaveChangesAsync();

                return "links_modified";
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("Openstack_panel", "projects", "ModifyProjectLinks", ex);
                return "error";
            }
        }

        public async Task<string> AddUser(addUserOb user, int projectId = -1, int uid = -1)
        {
            if (projectId == -1 || uid == -1)
                return "error";

            try
            {
                var userResult = await _users.AddUser(user, uid);

                if (userResult.result != "user_created")
                    return "error";

                _context.ProjectsUsers.Add(new Models.ProjectsUser
                {
                    ProjectId = (long)projectId,
                    UserId = (long)userResult.id
                });

                await _context.SaveChangesAsync();

                return "user_created";
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("Openstack_Panel", "projects", "AddUser", ex);
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
    }

    public class projectLinksDetailOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
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
