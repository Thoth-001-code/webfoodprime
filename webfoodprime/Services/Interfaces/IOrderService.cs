using webfoodprime.DTOs.Admin;
using webfoodprime.DTOs.Order;
using webfoodprime.DTOs.Staff;
using webfoodprime.Helpers.Enum;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IOrderService
    {//customer
        Task CreateOrder(string userId, CreateOrderDTO dto);

        Task<IEnumerable<OrderResponseDTO>> GetMyOrders(string userId);

        Task UpdateStatus(string adminUserId, UpdateOrderStatusDTO dto);

        Task CreateInStoreOrder(CreateInStoreOrderDTO dto);

        //ad    min
        Task<IEnumerable<OrderResponseDTO>> GetPendingOrders();
        Task ConfirmOrder(int orderId);
        Task ReadyOrder(int orderId);
        Task CancelOrder(int orderId);

        //admin & shipper
        Task<IEnumerable<AdminOrderDTO>> GetAllOrders(string? status);
        Task<IEnumerable<ShipperOrderDTO>> GetShipperOrders(string shipperId);

        //staff
        Task<List<StaffOrderDTO>> StaffGetPendingOrders();
        Task<List<StaffOrderDTO>> GetOrdersByStaff(string staffId);
        Task<StaffOrderDetailDTO> GetById(int id);
        Task AssignStaff(int orderId, string staffId);
        Task UpdateStatus(int orderId, OrderStatus status); // ✅ FIX
        // Staff can mark an order as paid (cash payment at counter)
        Task MarkAsPaid(int orderId, string staffId);
        // Staff-specific actions (ensure ownership)
        Task Confirm(int orderId, string staffId);
        Task Ready(int orderId, string staffId);
        Task Cancel(int orderId, string staffId);
    }
}
  