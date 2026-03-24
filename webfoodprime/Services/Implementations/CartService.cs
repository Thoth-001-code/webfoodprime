using Microsoft.EntityFrameworkCore;
using webfoodprime.DTOs.Cart;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;

namespace webfoodprime.Services.Implementations
{
    

    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToCart(string userId, AddToCartDTO dto)
        {
            // 🔥 Validate quantity
            if (dto.Quantity <= 0)
                throw new Exception("Quantity must be greater than 0");

            // 🔥 Check food tồn tại
            var food = await _context.Foods.FindAsync(dto.FoodId);
            if (food == null)
                throw new Exception("Food not found");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.FoodId == dto.FoodId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    FoodId = dto.FoodId,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCart(string userId, UpdateCartDTO dto)
        {
            var item = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == dto.CartItemId);

            if (item == null)
                throw new Exception("Item not found");

            // 🔥 CHECK OWNER
            if (item.Cart.UserId != userId)
                throw new Exception("Unauthorized");

            // 🔥 Quantity = 0 → xóa luôn
            if (dto.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItem(string userId, int cartItemId)
        {
            var item = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (item == null)
                throw new Exception("Item not found");

            // 🔥 CHECK OWNER
            if (item.Cart.UserId != userId)
                throw new Exception("Unauthorized");

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();
        }

        public async Task<Cart> GetCart(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Food)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart;
        }
    }


}
