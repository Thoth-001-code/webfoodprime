using webfoodprime.DTOs.Auth;

namespace webfoodprime.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterDTO model);
        Task<AuthResponseDTO> Login(LoginDTO model);
    }
}
 