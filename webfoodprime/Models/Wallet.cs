namespace webfoodprime.Models
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; } = 0; // ✅ thêm

        public ApplicationUser User { get; set; }
    }
}
