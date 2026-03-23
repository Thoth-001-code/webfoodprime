namespace webfoodprime.Models
{
    public class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }
    public Cart Cart { get; set; } // ✅ thêm

    public int FoodId { get; set; }
    public Food Food { get; set; } // ✅ thêm

    public int Quantity { get; set; }
}
}
