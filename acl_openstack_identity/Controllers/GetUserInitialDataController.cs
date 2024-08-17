using acl_openstack_identity.Data;
using acl_openstack_identity.features;
using acl_openstack_identity.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace acl_openstack_identity.Controllers
{
    [Route("api/usersData")]
    [ApiController]
    public class GetUserInitialDataController : ControllerBase
    {
        private readonly OpenstackContext _context;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;
        private hierarchy _hierarchy;
        private cookieToken _token;

        public GetUserInitialDataController(OpenstackContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
            _hierarchy = new hierarchy(context);
            _token = new cookieToken(_httpClient, _configuration);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserData()
        {
            var cookieToken = string.Empty;

            if (Request.Cookies["openstack_token"] != null)
            {
                cookieToken = Request.Cookies["openstack_token"];
            }

            if (string.IsNullOrEmpty(cookieToken))
                return StatusCode(401, new { result = "no_auth" });

            var userId = await _token.ValidateToken(cookieToken);

            if (userId.result == "error")
                return StatusCode(401, new { result = "no_auth" });

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId.userId);

                if (user == null)
                    return StatusCode(500, new { result = "error" });

                new UserDataOb
                {
                    email = user.Email,
                    name = user.Name,
                    lastname = user.Lastname,
                    phone = user.Phone,
                    id = (int)user.Id,
                    icon = user.Icon
                };

                return Ok(new
                {
                    result = "done",
                    user = user
                });
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("Openstack_Panel", "GetUserInitialDataController", "GetUserData", ex);
                return StatusCode(500, new { result = "error" });
            }
        }

        public async Task<IActionResult> GetUserOrganizations()
        {
            var cookieToken = string.Empty;

            if (Request.Cookies["openstack_token"] != null)
            {
                cookieToken = Request.Cookies["openstack_token"];
            }

            if (string.IsNullOrEmpty(cookieToken))
                return StatusCode(401, new { result = "no_auth" });

            var userId = await _token.ValidateToken(cookieToken);

            if (userId.result == "error")
                return StatusCode(401, new { result = "no_auth" });

            try
            {
                var organizations = _context.Projects
                    .AsNoTracking()
                    .Where(p => p.ParentId == null && p.ProjectsUsers.Any(pu => pu.ProjectId == p.Id && pu.UserId == userId.userId))
                    .Select(p => new UserDataProjectOb
                    {
                        id = (int)p.Id,
                        name = p.Name
                    }).ToList();

                return Ok(new
                {
                    result = "done",
                    organization = organizations
                });
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("Openstack_Panel", "GetUserInitialDataController", "GetUserOrganizations", ex);
                return StatusCode(500, new { result = "error" });
            }
        }
    }

    public class UserDataOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? icon { get; set; }
    }

    public class UserDataProjectOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
    }
}
