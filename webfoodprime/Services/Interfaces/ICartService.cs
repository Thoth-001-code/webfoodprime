using webfoodprime.DTOs.Cart;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCart(string userId, AddToCartDTO dto);

        Task UpdateCart(string userId, UpdateCartDTO dto);

        Task RemoveItem(string userId, int cartItemId);

        Task<Cart> GetCart(string userId);


    }
}
 