 using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using webfoodprime.DTOs.Address;
    using webfoodprime.Services.Interfaces;


namespace webfoodprime.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAddressDTO dto)
        {
            await _addressService.Create(GetUserId(), dto);
            return Ok("Created");
        }

        [HttpGet]
        public async Task<IActionResult> GetMy()
        {
            return Ok(await _addressService.GetMyAddresses(GetUserId()));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateAddressDTO dto)
        {
            await _addressService.Update(GetUserId(), dto);
            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _addressService.Delete(GetUserId(), id);
            return Ok("Deleted");
        }
    }
}
