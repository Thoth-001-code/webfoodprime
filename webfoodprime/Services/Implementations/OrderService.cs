
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

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateOrder(string userId, CreateOrderDTO dto)
        {
            // 🔥 1. Lấy Cart kèm món
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Food)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            // 🔥 2. Lấy Address
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == dto.AddressId && a.UserId == userId);

            if (address == null)
                throw new Exception("Address not found or does not belong to user");

            // 🔥 3. Tính tiền món
            decimal foodTotal = cart.CartItems.Sum(ci => ci.Quantity * ci.Food.Price);

            // 🔥 4. Tính phí ship (hardcode)
            decimal shippingFee = 15000;

            // 🔥 5. Tổng tiền
            decimal total = foodTotal + shippingFee;

            // 🔥 6. Kiểm tra ví
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null || wallet.Balance < total)
                throw new Exception("Not enough balance");

            // 🔥 7. Tạo Order
            var order = new Order
            {
                UserId = userId,
                AddressId = address.AddressId,
                Status = OrderStatus.Pending,
                FoodTotal = foodTotal,
                ShippingFee = shippingFee,
                TotalPrice = total,
                CreatedAt = DateTime.UtcNow
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // 🔹 cần save trước để có OrderId

            // 🔥 8. Tạo OrderDetails
            var details = cart.CartItems.Select(ci => new OrderDetail
            {
                OrderId = order.OrderId,
                FoodId = ci.FoodId,
                Quantity = ci.Quantity,
                Price = ci.Food.Price
            }).ToList();
            _context.OrderDetails.AddRange(details);

            // 🔥 9. Trừ tiền ví + tạo transaction
            wallet.Balance -= total;
            _context.Transactions.Add(new Transaction
            {
                WalletId = wallet.WalletId,
                Amount = total,
                Type = "Payment",
                CreatedAt = DateTime.UtcNow
            });

            // 🔥 10. Xóa CartItems
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetMyOrders(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Food)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateStatus(UpdateOrderStatusDTO dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);

            if (order == null)
                throw new Exception("Order not found");

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
