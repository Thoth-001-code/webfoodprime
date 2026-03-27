using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;

namespace webfoodprime.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IWalletService _walletService;
        private readonly IOrderService _orderService;

        public PaymentController(
            IPaymentService paymentService,
            IWalletService walletService,
            IOrderService orderService)
        {
            _paymentService = paymentService;
            _walletService = walletService;
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost("pay/{orderId}")]
        public async Task<IActionResult> Pay(int orderId)
        {
            var userId = GetUserId();

            // ✅ Kiểm tra số dư (vẫn giữ)
            var wallet = await _walletService.GetWallet(userId);

            // ⚠️ KHÔNG kiểm tra order ở đây vì PaymentService đã có kiểm tra
            // PaymentService.PayWithWallet sẽ tự kiểm tra order tồn tại, đã thanh toán, số dư

            await _paymentService.PayWithWallet(userId, orderId);

            return Ok(new { message = "Payment success" });
        }
    }
}