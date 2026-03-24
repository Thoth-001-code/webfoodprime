   using Microsoft.EntityFrameworkCore;
using webfoodprime.Helpers.Enum;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;


namespace webfoodprime.Services.Implementations
{
 

    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task PayWithWallet(string userId, int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.IsPaid)
                throw new Exception("Order already paid");

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                throw new Exception("Wallet not found");

            if (wallet.Balance < order.TotalPrice)
                throw new Exception("Not enough balance");

            // 🔥 trừ tiền
            wallet.Balance -= order.TotalPrice;

            // 🔥 ghi transaction
            _context.Transactions.Add(new Transaction
            {
                WalletId = wallet.WalletId,
                Amount = order.TotalPrice,
                Type = TransactionType.Payment,
                Description = $"Thanh toán đơn #{order.OrderId}",
                CreatedAt = DateTime.UtcNow
            });

            // 🔥 update order
            order.IsPaid = true;

            await _context.SaveChangesAsync();
        }
    }
}
