using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Authorization;
    
    using System.Security.Claims;
    using webfoodprime.DTOs.Wallet;
    using webfoodprime.Services.Interfaces;
namespace webfoodprime.Controllers
{


    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // 🔥 XEM VÍ
        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var wallet = await _walletService.GetWallet(userId);

            return Ok(wallet);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            return Ok(await _walletService.GetTransactions(GetUserId()));
        }

        // 🔥 NẠP TIỀN
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _walletService.Deposit(userId, dto.Amount);

            return Ok("Deposit success");
        }
    }
}
