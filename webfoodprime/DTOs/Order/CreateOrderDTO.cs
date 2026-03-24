using webfoodprime.Helpers.Enum;

namespace webfoodprime.DTOs.Order
{
    public class CreateOrderDTO
    {
        public int AddressId { get; set; }

        // 🔥 radio button FE sẽ gửi 1 hoặc 2
        public PaymentMethod PaymentMethod { get; set; }

        public string? Note { get; set; }
    }
}
