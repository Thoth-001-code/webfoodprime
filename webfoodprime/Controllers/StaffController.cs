using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Linq;
using webfoodprime.Services.Interfaces;
using webfoodprime.Helpers.Enum;
using webfoodprime.DTOs.Order;

namespace webfoodprime.Controllers
{
    [ApiController]
    [Route("api/staff")]
    [Authorize(Roles = "Staff")]
    public class StaffController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public StaffController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        /// 🔥 DANH SÁCH ĐƠN CHỜ NHẬN
        [HttpGet("orders")]
        public async Task<IActionResult> StaffGetPendingOrders()
        {
            return Ok(await _orderService.StaffGetPendingOrders());
        }

        // STAFF DASHBOARD - polling endpoint for dashboard (pending online + my orders)
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var pending = await _orderService.StaffGetPendingOrders();
            var myOrders = await _orderService.GetOrdersByStaff(GetUserId());

            return Ok(new
            {
                pendingOnlineOrders = pending,
                myOrders
            });
        }

        // 🔥 CHI TIẾT ĐƠN
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var data = await _orderService.GetById(id);
            return Ok(data);
        }

        // 🔥 NHẬN ĐƠN (assign staff)
        [HttpPut("orders/{id}/assign")]
        public async Task<IActionResult> Assign(int id)
        {
            await _orderService.AssignStaff(id, GetUserId());
            return Ok(new { message = "Assigned" });
        }

        // 🔥 XÁC NHẬN ĐƠN (bắt đầu làm)
        [HttpPut("orders/{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            await _orderService.Confirm(id, GetUserId());
            return Ok(new { message = "Confirmed" });
        }

        // 🔥 ĐÃ LÀM XONG
        [HttpPut("orders/{id}/ready")]
        public async Task<IActionResult> Ready(int id)
        {
            await _orderService.Ready(id, GetUserId());
            return Ok(new { message = "Ready" });
        }

        // 🔥 HỦY ĐƠN
        [HttpPut("orders/{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _orderService.Cancel(id, GetUserId());
            return Ok(new { message = "Cancelled" });
        }

        // 🔥 ĐƠN CỦA CHÍNH STAFF
        [HttpGet("my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            var data = await _orderService.GetOrdersByStaff(GetUserId());
            return Ok(data);
        }

        // POST: api/staff/instore - create in-store order (cashier)
        [HttpPost("instore")]
        public async Task<IActionResult> CreateInStore([FromBody] CreateInStoreOrderDTO dto)
        {
            await _orderService.CreateInStoreOrder(dto);
            return Ok(new { message = "Created" });
        }

        // POST: api/staff/pay/{orderId} - mark order as paid by staff (cash)
        [HttpPost("pay/{orderId}")]
        public async Task<IActionResult> Pay(int orderId)
        {
            await _orderService.MarkAsPaid(orderId, GetUserId());
            return Ok(new { message = "Paid" });
        }
    }
}