namespace webfoodprime.DTOs.Staff
{
    public class StaffOrderItemDTO
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        // subtotal = Quantity * Price
        public decimal SubTotal { get; set; }
    }
}