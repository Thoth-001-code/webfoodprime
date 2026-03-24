
using Microsoft.EntityFrameworkCore;
using webfoodprime.DTOs.Order;
using webfoodprime.Helpers.Enum;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;

 
namespace webfoodprime.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentService _paymentService;
        public OrderService(AppDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task CreateOrder(string userId, CreateOrderDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Address
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressId == dto.AddressId && a.UserId == userId);

                if (address == null)
                    throw new Exception("Invalid address");

                // 2. Cart
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Food)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.CartItems.Any())
                    throw new Exception("Cart is empty");

                // 3. Tính tiền
                decimal foodTotal = cart.CartItems.Sum(ci => ci.Quantity * ci.Food.Price);

                var shipping = await _context.ShippingFees.FirstOrDefaultAsync();
                decimal shippingFee = shipping?.Fee ?? 15000;

                decimal total = foodTotal + shippingFee;

                // 4. Tạo Order
                var order = new Order
                {
                    UserId = userId,
                    AddressId = dto.AddressId,
                    Note = dto.Note,
                    Status = OrderStatus.Pending,
                    FoodTotal = foodTotal,
                    ShippingFee = shippingFee,
                    TotalPrice = total,
                    PaymentMethod = dto.PaymentMethod,
                    IsPaid = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 5. OrderDetails
                var details = cart.CartItems.Select(ci => new OrderDetail
                {
                    OrderId = order.OrderId,
                    FoodId = ci.FoodId,
                    Quantity = ci.Quantity,
                    Price = ci.Food.Price
                });

                _context.OrderDetails.AddRange(details);

                // 6. PAYMENT LOGIC 🔥
                if (dto.PaymentMethod == PaymentMethod.Wallet)
                {
                    await _paymentService.PayWithWallet(userId, order.OrderId);
                }
                // COD → không làm gì

                // 7. Clear cart
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetMyOrders(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Food)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // 🔥 MAP DTO (QUAN TRỌNG)
            return orders.Select(o => new OrderResponseDTO
            {
                OrderId = o.OrderId,
                TotalPrice = o.TotalPrice,
                FoodTotal = o.FoodTotal,
                ShippingFee = o.ShippingFee,
                Status = o.Status.ToString(),
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

        public async Task UpdateStatus(int adminUserId, UpdateOrderStatusDTO dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);

            if (order == null)
                throw new Exception("Order not found");

            // 🔥 VALIDATE FLOW TRẠNG THÁI
            var valid = order.Status switch
            {
                OrderStatus.Pending => dto.Status == OrderStatus.Confirmed,
                OrderStatus.Confirmed => dto.Status == OrderStatus.Preparing,
                OrderStatus.Preparing => dto.Status == OrderStatus.Ready,
                _ => false
            };

            if (!valid)
                throw new Exception("Invalid status transition");

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
