
using Microsoft.EntityFrameworkCore;
using webfoodprime.DTOs.Admin;
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

            var valid = order.Status switch
            {
                OrderStatus.Pending => dto.Status == OrderStatus.Confirmed || dto.Status == OrderStatus.Cancelled,

                OrderStatus.Confirmed => dto.Status == OrderStatus.Ready || dto.Status == OrderStatus.Cancelled,

                OrderStatus.Ready => dto.Status == OrderStatus.Delivering,

                OrderStatus.Delivering => dto.Status == OrderStatus.Completed,

                _ => false
            };

            if (!valid)
                throw new Exception("Invalid status transition");

            // 🔥 BONUS 1: COD → auto paid khi completed
            if (dto.Status == OrderStatus.Completed)
            {
                if (order.PaymentMethod == PaymentMethod.COD)
                {
                    order.IsPaid = true;
                }
            }

            // 🔥 BONUS 2: refund khi cancel
            if (dto.Status == OrderStatus.Cancelled)
            {
                if (order.IsPaid && order.PaymentMethod == PaymentMethod.Wallet)
                {
                    var wallet = await _context.Wallets
                        .FirstOrDefaultAsync(w => w.UserId == order.UserId);

                    if (wallet == null)
                        throw new Exception("Wallet not found");

                    wallet.Balance += order.TotalPrice;

                    _context.Transactions.Add(new Transaction
                    {
                        WalletId = wallet.WalletId,
                        Amount = order.TotalPrice,
                        Type = TransactionType.Deposit,
                        Description = "Refund order",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        public async Task CreateInStoreOrder(CreateInStoreOrderDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (dto.Items == null || !dto.Items.Any())
                    throw new Exception("No items");

                // 1. Lấy danh sách food
                var foodIds = dto.Items.Select(i => i.FoodId).ToList();

                var foods = await _context.Foods
                    .Where(f => foodIds.Contains(f.FoodId))
                    .ToListAsync();

                if (foods.Count != dto.Items.Count)
                    throw new Exception("Some foods not found");

                // 2. Tính tiền
                decimal total = 0;

                var orderDetails = new List<OrderDetail>();

                foreach (var item in dto.Items)
                {
                    var food = foods.First(f => f.FoodId == item.FoodId);

                    var price = food.Price;
                    total += price * item.Quantity;

                    orderDetails.Add(new OrderDetail
                    {
                        FoodId = food.FoodId,
                        Quantity = item.Quantity,
                        Price = price
                    });
                }

                // 3. Tạo Order (🔥 KHÁC ONLINE)
                var order = new Order
                {
                    OrderType = OrderType.InStore,
                    Status = OrderStatus.Completed,

                    PaymentMethod = dto.PaymentMethod, // 🔥 thêm

                    IsPaid = true, // mặc định đã trả tại quầy
                    PaidAt = DateTime.UtcNow,

                    FoodTotal = total,
                    ShippingFee = 0,
                    TotalPrice = total,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 4. Gán OrderId
                foreach (var d in orderDetails)
                {
                    d.OrderId = order.OrderId;
                }

                _context.OrderDetails.AddRange(orderDetails);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<OrderResponseDTO>> GetPendingOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                .Where(o => o.Status == OrderStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

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
        public async Task ConfirmOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Only Pending can be confirmed");

            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        public async Task ReadyOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Confirmed)
                throw new Exception("Must be Confirmed first");

            order.Status = OrderStatus.Ready;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task CancelOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status == OrderStatus.Completed)
                throw new Exception("Cannot cancel completed order");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            // 🔥 refund tại đây (DUY NHẤT)
            if (order.IsPaid && order.PaymentMethod == PaymentMethod.Wallet)
            {
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == order.UserId);

                if (wallet != null)
                {
                    wallet.Balance += order.TotalPrice;

                    _context.Transactions.Add(new Transaction
                    {
                        WalletId = wallet.WalletId,
                        Amount = order.TotalPrice,
                        Type = TransactionType.Deposit,
                        Description = "Refund order",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<AdminOrderDTO>> GetAllOrders(string? status)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                .Include(o => o.Address)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<OrderStatus>(status, true, out var parsed))
                {
                    query = query.Where(o => o.Status == parsed);
                }
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new AdminOrderDTO
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                Address = o.Address != null ? o.Address.FullAddress : "InStore",

                Status = o.Status.ToString(),
                OrderType = o.OrderType.ToString(),

                FoodTotal = o.FoodTotal,
                ShippingFee = o.ShippingFee,
                TotalPrice = o.TotalPrice,

                PaymentMethod = o.PaymentMethod.ToString(),
                IsPaid = o.IsPaid,

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

    }//////////
} 
