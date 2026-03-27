  using global::webfoodprime.DTOs.Order;
    using global::webfoodprime.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            private readonly IOrderService _orderService;
            public ShipperController(IShippingService shippingService, IOrderService orderService)  
            {
                _shippingService = shippingService;
                _orderService = orderService;
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
            // 🔥 GET: api/Shipper/my-orders
            [HttpGet("my-orders")]
            [Authorize(Roles = "Shipper")]
            public async Task<IActionResult> GetMyOrders()
            {
                var shipperId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (shipperId == null)
                    return Unauthorized();

                var orders = await _orderService.GetShipperOrders(shipperId);

                return Ok(orders);
            }
        }
    } 
}
 