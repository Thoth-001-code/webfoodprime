using webfoodprime.DTOs.Cart;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserId(string userId);
        Task AddToCart(string userId, AddToCartDTO dto);
        Task UpdateCartItem(string userId, UpdateCartDTO dto);
        Task RemoveCartItem(string userId, int cartItemId);


    }
}
 