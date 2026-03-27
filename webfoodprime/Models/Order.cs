using webfoodprime.Helpers.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webfoodprime.Models
{


    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // 🔥 phân loại đơn
        public OrderType OrderType { get; set; } = OrderType.Delivery;

        // 🔷 Customer (nullable cho InStore)
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // 🔷 Address (nullable cho InStore)
        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        public string? Note { get; set; }

        // 🔷 Shipper (chỉ dùng cho Delivery)
        public string? ShipperId { get; set; }
        public ApplicationUser? Shipper { get; set; }
        // 🔷 PaymentMethod (nullable cho InStore)
        public PaymentMethod PaymentMethod { get; set; }
        // 🔷 Staff (chỉ dùng cho InStore)
        public string? StaffId { get; set; }
       
        // 🔷 IsPaid (mặc định false, chỉ true khi đã thanh toán)
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal FoodTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
