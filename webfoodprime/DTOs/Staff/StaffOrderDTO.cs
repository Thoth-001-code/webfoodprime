namespace webfoodprime.DTOs.Staff
{
    public class StaffOrderDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        // Additional fields for staff dashboard
        public string OrderType { get; set; }     // InStore / Delivery
        public bool IsPaid { get; set; }          // paid or not
        public string PaymentMethod { get; set; } // COD / Wallet / Cash
        public string Note { get; set; }          // customer note

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int ItemCount { get; set; }
        public string Address { get; set; }
        public string StaffId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal FoodTotal { get; set; }
        public decimal ShippingFee { get; set; }
    }
}