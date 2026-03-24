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

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(AddToCartDTO dto)
        {
            await _cartService.AddToCart(GetUserId(), dto);
            return Ok("Added");
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateCartDTO dto)
        {
            await _cartService.UpdateCart(GetUserId(), dto);
            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            await _cartService.RemoveItem(GetUserId(), id);
            return Ok("Removed");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _cartService.GetCart(GetUserId()));
        }
    }
}
