
  using Microsoft.EntityFrameworkCore;
    using webfoodprime.DTOs.Order;
    using webfoodprime.Helpers.Enum;
    using webfoodprime.Models;
    using webfoodprime.Services.Interfaces;


namespace webfoodprime.Services.Implementations
{
  

    public class ShippingService : IShippingService
    { 
        private readonly AppDbContext _context;

        public ShippingService(AppDbContext context)
        {
            _context = context;
        }

        // 🔥 Lấy đơn chưa có shipper
        public async Task<IEnumerable<ShipperOrderDTO>> GetAvailableOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                .Where(o => o.Status == OrderStatus.Ready
                            && o.ShipperId == null
                            && o.OrderType == OrderType.Delivery)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new ShipperOrderDTO
            {
                OrderId = o.OrderId,
                Address = o.Address.FullAddress,
                TotalPrice = o.TotalPrice,
                PaymentMethod = o.PaymentMethod.ToString(),
                Note = o.Note,
                CreatedAt = o.CreatedAt,

                Items = o.OrderDetails.Select(od => new OrderDetailDTO
                {
                    FoodName = od.Food.FoodName,
                    Price = od.Price,
                    Quantity = od.Quantity
                }).ToList()
            });
        }
        // 🔥 Shipper nhận đơn
        public async Task TakeOrder(string shipperId, int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Ready)
                throw new Exception("Order not ready");

            if (order.ShipperId != null)
                throw new Exception("Order already taken");

            order.ShipperId = shipperId;
            order.Status = OrderStatus.Delivering; // 🔥 đúng

            await _context.SaveChangesAsync();
        }

        // 🔥 Update trạng thái
        public async Task UpdateStatus(string shipperId, UpdateOrderStatusDTO dto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.ShipperId != shipperId)
                throw new Exception("Not your order");

            // ❗ Không cho update linh tinh
            if (dto.Status == OrderStatus.Delivering)
                throw new Exception("Already delivering");

            if (dto.Status == OrderStatus.Completed)
            {
                if (order.Status != OrderStatus.Delivering)
                    throw new Exception("Must be delivering first");

                // 🔥 COD → auto paid
                if (order.PaymentMethod == PaymentMethod.COD)
                {
                    order.IsPaid = true;
                    order.PaidAt = DateTime.UtcNow;
                }

                order.Status = OrderStatus.Completed;
                order.DeliveredAt = DateTime.UtcNow;
            }
            else
            {
                throw new Exception("Invalid status");
            }

            await _context.SaveChangesAsync();
        }
    }
}
