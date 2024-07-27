using acl_openstack_authorization.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace acl_openstack_authorization.Controllers
{
    [Route("/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthenticationController(IConfiguration configuration, ITokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost("generate-token")]
        public IActionResult GenerateToken([FromBody] TokenRequest request)
        {
            var token = _tokenService.GenerateJwtToken(request.userId);
            return new JsonResult(new { result = "done", token = token });
        }

        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] TokenValidationRequest request)
        {
            var principal = _tokenService.GetPrincipalFromToken(request.Token);

            if (principal == null)
                return new JsonResult(new { result = "unathenticated" });

            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new JsonResult(new { result = "unauthenticated" });

            return new JsonResult(new
            {
                result = "authenticated",
                userId = Convert.ToInt32(userIdClaim.Value)
            });
        }

        [HttpPost("renew-token")]
        public async Task<IActionResult> RenewToken([FromBody] TokenValidationRequest request)
        {
            var principal = _tokenService.GetPrincipalFromToken(request.Token);

            if (principal == null)
                return new JsonResult(new { result = "unauthenticated" });

            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new JsonResult(new { result = "unauthenticated" });

            await _tokenService.InvalidateToken(request.Token);

            var newToken = _tokenService.GenerateJwtToken(int.Parse(userIdClaim.Value));

            return new JsonResult(new
            {
                result = "done",
                token = newToken
            });
        }
    }

    public class TokenRequest
    {
        public int userId { get; set; }
    }

    public class  TokenValidationRequest
    {
        public string Token { get; set; }
    }
}
