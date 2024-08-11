/*using acl_openstack_identity.Data;
using acl_openstack_identity.features;
using acl_openstack_identity.Helpers;
using acl_openstack_identity.Resources;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
namespace acl_openstack_identity.Controllers
{
    [Route("api/organizations")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private assecoOpenstackContext _context;
        private organizations organizationsController;
        private readonly IConfiguration _configuration;

        public OrganizationController(assecoOpenstackContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            organizationsController = new organizations(configuration, context, httpClient);
            _configuration = configuration;
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrganization([FromBody]AddOrganizationOb organization)
        {

            try
            {
                var result = await organizationsController.CreateOrganization(organization);
                switch (result.result)
                {
                    case "error":
                        return StatusCode(500, new { result = result.result });
                        break;
                    case "no_name":
                        return BadRequest(new {result = result.result});
                        break;
                    case "organization_created":
                        return Ok(new {result = result.result});
                        break;
                    default:
                        return StatusCode(500, new { result = result.result });
                        break;
                }
            }
            catch (NpgsqlException ex)
            {
                Logger.SendException("acl_openstack", "OrganizationController", "CreateOrganization", ex);
                return StatusCode(500, new { result = $"error {ex}" });
            }
        }
    }
}
*/