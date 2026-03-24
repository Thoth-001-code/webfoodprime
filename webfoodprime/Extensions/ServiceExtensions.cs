using webfoodprime.Services.Implementations;
using webfoodprime.Services.Interfaces;

namespace webfoodprime.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthService, AuthService>(); services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICartService, CartService>(); services.AddScoped<IAddressService, AddressService>(); 
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IOrderService, OrderService>();
           
            




        }
    }
}
