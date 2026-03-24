namespace webfoodprime.DTOs.Order
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FoodTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<OrderDetailDTO> Items { get; set; }
    }
}
