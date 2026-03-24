namespace webfoodprime.Models
{
    public class Cart
{
    public int CartId { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; } // ✅ thêm

        public List<CartItem> CartItems { get; set; } = new();
    }
}
