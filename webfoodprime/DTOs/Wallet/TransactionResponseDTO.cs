namespace webfoodprime.DTOs.Wallet
{
    public class TransactionResponseDTO
    {
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
