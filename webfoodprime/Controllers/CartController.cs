using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using webfoodprime.DTOs.Cart;
using webfoodprime.Services.Interfaces;

   
namespace webfoodprime.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartByUserId(GetUserId());
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartDTO dto)
        {
            await _cartService.AddToCart(GetUserId(), dto);
            return Ok(new { message = "Added to cart" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem(UpdateCartDTO dto)
        {
            await _cartService.UpdateCartItem(GetUserId(), dto);
            return Ok(new { message = "Cart updated" });
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            await _cartService.RemoveCartItem(GetUserId(), cartItemId);
            return Ok(new { message = "Item removed" });
        }
    }
}
