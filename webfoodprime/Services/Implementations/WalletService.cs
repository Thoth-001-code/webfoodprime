 using Microsoft.EntityFrameworkCore;
using webfoodprime.DTOs.Wallet;
using webfoodprime.Helpers.Enum;
using webfoodprime.Models;
    using webfoodprime.Services.Interfaces;


namespace webfoodprime.Services.Implementations
{
   

    public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;

        public WalletService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> GetWallet(string userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            // 🔥 chưa có ví → tạo luôn
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    Balance = 0
                };

                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            return wallet;
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetTransactions(string userId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                throw new Exception("Wallet not found");

            var transactions = await _context.Transactions
                .Where(t => t.WalletId == wallet.WalletId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // 🔥 MAP DTO Ở ĐÂY
            return transactions.Select(t => new TransactionResponseDTO
            {
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Description = t.Description,
                CreatedAt = t.CreatedAt
            });
        }
        public async Task Deposit(string userId, decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Amount must be greater than 0");

            var wallet = await GetWallet(userId);

            wallet.Balance += amount;

            _context.Transactions.Add(new Transaction
            {
                WalletId = wallet.WalletId,
                Amount = amount,
                Type = TransactionType.Deposit,
                Description = "Nạp tiền"
            });

            await _context.SaveChangesAsync();
        }
    }
}
