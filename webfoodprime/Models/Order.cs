using webfoodprime.Helpers.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webfoodprime.Models
{


    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // 🔷 Người đặt hàng (Customer)
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // 🔷 Địa chỉ giao hàng
        [Required]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        // 🔷 Shipper (có thể null)
        public string? ShipperId { get; set; }
        public ApplicationUser? Shipper { get; set; }

        // 🔥 Trạng thái đơn hàng (DÙNG ENUM)
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // 💰 Tiền
        [Column(TypeName = "decimal(18,2)")]
        public decimal FoodTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // ⏱ Thời gian
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        // 📄 Chi tiết đơn hàng
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
