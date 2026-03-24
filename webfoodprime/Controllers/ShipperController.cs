using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
  using global::webfoodprime.DTOs.Order;
    using global::webfoodprime.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Claims;
  
namespace webfoodprime.Controllers
{
  
     
    namespace webfoodprime.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        [Authorize(Roles = "Shipper")]
        public class ShipperController : ControllerBase
        {
            private readonly IShippingService _shippingService;

            public ShipperController(IShippingService shippingService)
            {
                _shippingService = shippingService;
            }

            private string GetUserId()
            {
                return User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            // 🔥 GET: api/Shipper/available-orders
            [HttpGet("available-orders")]
            public async Task<IActionResult> GetAvailableOrders()
            {
                var orders = await _shippingService.GetAvailableOrders();
                return Ok(orders);
            }

            // 🔥 POST: api/Shipper/take/5
            [HttpPost("take/{orderId}")]
            public async Task<IActionResult> TakeOrder(int orderId)
            {
                await _shippingService.TakeOrder(GetUserId(), orderId);
                return Ok("Order taken");
            }

            // 🔥 PUT: api/Shipper/status
            [HttpPut("status")]
            public async Task<IActionResult> UpdateStatus(UpdateOrderStatusDTO dto)
            {
                await _shippingService.UpdateStatus(GetUserId(), dto);
                return Ok("Status updated");
            }
        }
    }
}
 