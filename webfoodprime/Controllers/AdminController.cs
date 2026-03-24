using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webfoodprime.DTOs.Order;
using webfoodprime.Services.Implementations;
using webfoodprime.Services.Interfaces;

namespace webfoodprime.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IOrderService _orderService;

        public AdminController(IAdminService adminService, IOrderService orderService)
        {
            _adminService = adminService;
            _orderService = orderService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var data = await _adminService.GetDashboard();
            return Ok(data);
        }
       
        [AllowAnonymous] // 🔥 cho khách xem
        [HttpGet("top-foods")]
        public async Task<IActionResult> GetTopFoods([FromQuery] int top = 5)
        {
            var data = await _adminService.GetTopFoods(top);
            return Ok(data);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOrders()
        {
            return Ok(await _orderService.GetPendingOrders());
        }

        [HttpPost("confirm/{orderId}")]
        public async Task<IActionResult> Confirm(int orderId)
        {
            await _orderService.ConfirmOrder(orderId);
            return Ok("Confirmed");
        }

        [HttpPost("ready/{orderId}")]
        public async Task<IActionResult> Ready(int orderId)
        {
            await _orderService.ReadyOrder(orderId);
            return Ok("Ready");
        }

        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> Cancel(int orderId)
        {
            await _orderService.CancelOrder(orderId);
            return Ok("Cancelled");
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] string? status)
        {
            return Ok(await _orderService.GetAllOrders(status));
        }

    }/////
}
