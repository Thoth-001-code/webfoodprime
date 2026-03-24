using webfoodprime.DTOs.Order;

namespace webfoodprime.DTOs.Admin
{
    public class AdminOrderDTO
    {
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public string? Address { get; set; }

        public string Status { get; set; }
        public string OrderType { get; set; }

        public decimal FoodTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderDetailDTO> Items { get; set; }
    }
}
