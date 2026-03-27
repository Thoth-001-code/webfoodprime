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

        // 🔥 Customer - THÊM Authorize
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(CreateOrderDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _orderService.CreateOrder(userId, dto);

            return Ok("Order created");
        }

        // 🔥 Customer - THÊM Authorize
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orders = await _orderService.GetMyOrders(userId);

            return Ok(orders);
        }

        // 🔥 Admin - Giữ nguyên
        [HttpPut("status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusDTO dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _orderService.UpdateStatus(adminId, dto);

            return Ok("Updated");
        }

        // 🔥 Admin - Giữ nguyên
        [HttpPost("instore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateInStoreOrder(CreateInStoreOrderDTO dto)
        {
            await _orderService.CreateInStoreOrder(dto);
            return Ok("In-store order created");
        }
    }//
}