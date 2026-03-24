using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IPaymentService
    {
        Task PayWithWallet(string userId, int order);
    }
}
