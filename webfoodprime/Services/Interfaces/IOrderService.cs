using webfoodprime.DTOs.Order;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrder(string userId, CreateOrderDTO dto);
        Task<List<Order>> GetMyOrders(string userId);
        Task UpdateStatus(UpdateOrderStatusDTO dto);
    }
}
