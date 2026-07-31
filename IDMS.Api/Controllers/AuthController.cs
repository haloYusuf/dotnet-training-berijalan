using IDMS.Api.Helpers;
using IDMS.Modules.Api.Master.Dto.Request.Auth;
using IDMS.Modules.Api.Master.Services;
using Microsoft.AspNetCore.Mvc;

namespace IDMS.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ReqMstUserCreateDto req)
        {
            await _service.registerAsync(req);
            return Ok(ApiResponseHelper.Success(HttpContext, (object?)null, "User registered successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ReqLoginDto req)
        {
            var data = await _service.loginAsync(req);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "User Login success"));
        }
    }
}