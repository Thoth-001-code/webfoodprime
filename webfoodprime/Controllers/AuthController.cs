using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webfoodprime.DTOs.Auth;
using webfoodprime.Services.Interfaces;

namespace webfoodprime.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            var result = await _authService.Register(model);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var token = await _authService.Login(model);
            return Ok(new { token });
        }
    }
}
