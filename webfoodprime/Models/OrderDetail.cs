namespace webfoodprime.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; }
        public Food Food { get; set; }    
        public string? Note { get; set; } // ví dụ: "ít đá", "thêm cơm"
    }
}
