namespace webfoodprime.DTOs.Order
{
    public class InStoreItemDTO
    {
        public int FoodId { get; set; }
        public int Quantity { get; set; }

        // 🔥 custom của khách
        public string? Note { get; set; }
    }
}
