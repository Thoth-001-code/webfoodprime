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

        public async Task<Cart> GetCartByUserId(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Food)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task AddToCart(string userId, AddToCartDTO dto)
        {
            var cart = await GetCartByUserId(userId);

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.FoodId == dto.FoodId);
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

        public async Task UpdateCartItem(string userId, UpdateCartDTO dto)
        {
            var cart = await GetCartByUserId(userId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == dto.CartItemId);

            if (item == null) throw new Exception("Item not found in cart");
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

        public async Task RemoveCartItem(string userId, int cartItemId)
        {
            var cart = await GetCartByUserId(userId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (item == null) throw new Exception("Item not found in cart");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }


}
