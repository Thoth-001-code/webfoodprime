namespace webfoodprime.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // ✅ phải có

        public string FullAddress { get; set; }
    }
}
