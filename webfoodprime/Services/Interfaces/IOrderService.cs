using webfoodprime.DTOs.Order;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrder(string userId, CreateOrderDTO dto);

        Task<IEnumerable<OrderResponseDTO>> GetMyOrders(string userId);

        Task UpdateStatus(int adminUserId, UpdateOrderStatusDTO dto);
    }
}
 