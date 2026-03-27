namespace webfoodprime.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        public string UserId { get; set; }

        public string FullAddress { get; set; }
        public string Phone { get; set; } // 🔥 thêm 

        public string? Note { get; set; }

        public ApplicationUser User { get; set; }
    }
}
