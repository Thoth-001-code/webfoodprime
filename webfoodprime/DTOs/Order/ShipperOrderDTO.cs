namespace webfoodprime.DTOs.Order
{
    public class ShipperOrderDTO
    {
        public int OrderId { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; } // 🔥 thêm

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } // 🔥 thêm

        public string PaymentMethod { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderDetailDTO> Items { get; set; }
    }
}
