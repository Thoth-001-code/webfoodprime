using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Authorization;
   
    using System.Security.Claims;
    using webfoodprime.DTOs.Order;
    using webfoodprime.Services.Interfaces;

namespace webfoodprime.Controllers
{
  

    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // 🔥 Customer
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(CreateOrderDTO dto)
        {
            await _orderService.CreateOrder(GetUserId(), dto);
            return Ok("Order created");
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyOrders()
        {
            return Ok(await _orderService.GetMyOrders(GetUserId()));
        }

        // 🔥 Admin
        [HttpPut("status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusDTO dto)
        {
            await _orderService.UpdateStatus(dto);
            return Ok("Updated");
        }
    }
}
