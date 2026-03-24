using webfoodprime.Helpers.Enum;

namespace webfoodprime.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public int WalletId { get; set; }

        public decimal Amount { get; set; }

        public TransactionType Type { get; set; } // ✅ đổi từ string → enum


        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Wallet Wallet { get; set; }
    }
} 
  