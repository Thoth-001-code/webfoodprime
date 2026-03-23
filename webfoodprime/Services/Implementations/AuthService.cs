using Microsoft.AspNetCore.Identity;
using webfoodprime.DTOs.Auth;
using webfoodprime.Helpers;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;
namespace webfoodprime.Services.Implementations
{


    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtHelper _jwtHelper;

        // 🔥 CONSTRUCTOR (BẮT BUỘC)
        public AuthService(
            UserManager<ApplicationUser> userManager,
            JwtHelper jwtHelper)
        {
            _userManager = userManager;
            _jwtHelper = jwtHelper;
        }

        public async Task<string> Register(RegisterDTO model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return "Register success";
        }

        public async Task<string> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                throw new Exception("User not found");

            var check = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!check)
                throw new Exception("Wrong password");

            var roles = await _userManager.GetRolesAsync(user);

            return _jwtHelper.GenerateToken(user, roles);
        }
    }
}
