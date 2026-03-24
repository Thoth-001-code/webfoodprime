using webfoodprime.DTOs.Wallet;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> GetWallet(string userId);


        Task<IEnumerable<TransactionResponseDTO>> GetTransactions(string userId);

        Task Deposit(string userId, decimal amount);
    }
}
 