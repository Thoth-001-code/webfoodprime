using Microsoft.AspNetCore.Identity;
using webfoodprime.DTOs.Auth;
using webfoodprime.Helpers;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;

    using Microsoft.EntityFrameworkCore;

namespace webfoodprime.Services.Implementations
{


 
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtHelper _jwtHelper;
        private readonly AppDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            JwtHelper jwtHelper,
            AppDbContext context)
        {
            _userManager = userManager;
            _jwtHelper = jwtHelper;
            _context = context;
        }

        public async Task<string> Register(RegisterDTO model)
        {
            var exist = await _userManager.FindByEmailAsync(model.Email);
            if (exist != null)
                throw new Exception("Email already exists");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // 🔥 Gán role Customer
            await _userManager.AddToRoleAsync(user, "Customer");

            // 🔥 TẠO WALLET + CART (CỰC QUAN TRỌNG)
            _context.Wallets.Add(new Wallet
            {
                UserId = user.Id,
                Balance = 0
            });

            _context.Carts.Add(new Cart
            {
                UserId = user.Id
            });

            await _context.SaveChangesAsync();

            return "Register success";
        }

        public async Task<AuthResponseDTO> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                throw new Exception("User not found");

            var check = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!check)
                throw new Exception("Wrong password");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtHelper.GenerateToken(user, roles);

            return new AuthResponseDTO
            {
                Token = token,
                Email = user.Email,
                Roles = roles.ToList()
            };
        }
    }
}
