using webfoodprime.Helpers.Enum;

namespace webfoodprime.DTOs.Order
{
    public class CreateInStoreOrderDTO
    {
        public List<InStoreItemDTO> Items { get; set; }
        // 🔥 thêm dòng này
        public PaymentMethod PaymentMethod { get; set; }
    }
}
