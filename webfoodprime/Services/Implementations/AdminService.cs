


using Microsoft.EntityFrameworkCore;
using webfoodprime.DTOs.Admin;
using webfoodprime.DTOs.Order;
using webfoodprime.Helpers.Enum;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;

namespace webfoodprime.Services.Implementations
{

    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDTO> GetDashboard()
        {
            var dashboard = new DashboardDTO
            {
                TotalRevenue = await _context.Orders
         .Where(o => o.Status == OrderStatus.Completed)
         .SumAsync(o => (decimal?)o.TotalPrice) ?? 0,

                TotalOrders = await _context.Orders.CountAsync(),

                PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending),

                ConfirmedOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Confirmed),

                ReadyOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Ready), // 🔥 thêm

                DeliveringOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Delivering),

                CompletedOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Completed),

                CancelledOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled),

                RevenueByDate = await _context.Orders
         .Where(o => o.Status == OrderStatus.Completed)
         .GroupBy(o => o.CreatedAt.Date)
         .Select(g => new RevenueByDateDTO
         {
             Date = g.Key,
             Revenue = g.Sum(x => x.TotalPrice)
         })
         .OrderBy(x => x.Date)
         .ToListAsync()
            };

            return dashboard;
        }
        public async Task<IEnumerable<TopFoodDTO>> GetTopFoods(int top = 5)
        {
            return await _context.OrderDetails
                .Where(od =>
                    od.Order.Status == OrderStatus.Completed &&
                    od.Order.IsPaid
                )
                .GroupBy(od => new
                {
                    od.FoodId,
                    od.Food.FoodName,
                    od.Food.ImagePath,
                    od.Food.Price
                })
                .Select(g => new TopFoodDTO
                {
                    FoodId = g.Key.FoodId,
                    FoodName = g.Key.FoodName,
                    ImageUrl = g.Key.ImagePath,
                    Price = g.Key.Price,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(top)
                .ToListAsync();
        }



    }////
}
