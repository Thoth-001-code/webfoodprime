using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webfoodprime.DTOs.Food;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;
namespace webfoodprime.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        // 🔓 PUBLIC
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return Ok(await _foodService.GetAll(baseUrl));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return Ok(await _foodService.GetById(id, baseUrl));
        }

        // 🔒 ADMIN CREATE
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateFoodDTO dto)
        {
            string imagePath = null;

            if (dto.Image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                imagePath = "/images/" + fileName;
            }

            await _foodService.Create(dto, imagePath);

            return Ok("Created");
        }

        // 🔥 UPDATE (CÓ UPLOAD ẢNH)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] CreateFoodDTO dto)
        {
            string imagePath = null;

            if (dto.Image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                imagePath = "/images/" + fileName;
            }

            await _foodService.Update(id, dto, imagePath);

            return Ok("Updated");
        }

        // 🔥 DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _foodService.Delete(id);
            return Ok("Deleted");
        }
    }
}
