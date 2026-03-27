  using webfoodprime.DTOs.Order;
    using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
  

    public interface IShippingService
    {
        Task<IEnumerable<ShipperOrderDTO>> GetAvailableOrders();
        Task TakeOrder(string shipperId, int orderId);
        Task UpdateStatus(string shipperId, UpdateOrderStatusDTO dto);
        Task<IEnumerable<ShipperOrderDTO>> GetShipperOrders(string shipperId);
    }
}
   