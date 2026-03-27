using webfoodprime.DTOs.Admin;
using webfoodprime.DTOs.Order;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrder(string userId, CreateOrderDTO dto);

        Task<IEnumerable<OrderResponseDTO>> GetMyOrders(string userId);

        Task UpdateStatus(int adminUserId, UpdateOrderStatusDTO dto);

        Task CreateInStoreOrder(CreateInStoreOrderDTO dto);

        //ad    min
        Task<IEnumerable<OrderResponseDTO>> GetPendingOrders();
        Task ConfirmOrder(int orderId);
        Task ReadyOrder(int orderId);
        Task CancelOrder(int orderId);

        Task<IEnumerable<AdminOrderDTO>> GetAllOrders(string? status);
        Task<IEnumerable<ShipperOrderDTO>> GetShipperOrders(string shipperId);



    }
}
  