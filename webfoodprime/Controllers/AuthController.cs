using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webfoodprime.DTOs.Auth;
using webfoodprime.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
   
    using System.Security.Claims;
    using webfoodprime.Models;


namespace webfoodprime.Controllers
{
    

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
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
            var result = await _authService.Login(model);
            return Ok(result);
        }

        // 🔥 API QUAN TRỌNG
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Email,
                roles
            });
        }
    }
}
