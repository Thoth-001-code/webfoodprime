using webfoodprime.Services.Implementations;
using webfoodprime.Services.Interfaces;

namespace webfoodprime.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICartService, CartService>();
        }
    }
}
