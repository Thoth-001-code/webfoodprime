using webfoodprime.Helpers.Enum;

namespace webfoodprime.DTOs.Order
{
    public class UpdateOrderStatusDTO
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
    }
}
