using System;
using System.Collections.Generic;

namespace webfoodprime.DTOs.Staff
{
    public class StaffOrderDetailDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }

        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        public string Address { get; set; }
        public string Note { get; set; }

        public string OrderType { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }

        public decimal FoodTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }

        public string StaffId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public List<StaffOrderItemDTO> Items { get; set; }
    }
}
