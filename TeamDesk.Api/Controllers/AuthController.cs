using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamDesk.Api.Dtos.Auth;
using TeamDesk.Api.Services;

namespace TeamDesk.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("login")]
        [AllowAnonymous]                                         // accessible sans token
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest dto)
        {
            var result = await _auth.LoginAsync(dto);
            return result is null ? Unauthorized() : Ok(result); // 401 sinon 200
        }
    }
}
