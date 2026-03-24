using Microsoft.EntityFrameworkCore;
using webfoodprime.DTOs.Food;
using webfoodprime.Models;
using webfoodprime.Services.Interfaces;

namespace webfoodprime.Services.Implementations
{

    public class FoodService : IFoodService
    {
        private readonly AppDbContext _context;

        public FoodService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FoodResponseDTO>> GetAll(string baseUrl)
        {
            return await _context.Foods
                .Select(f => new FoodResponseDTO
                {
                    FoodId = f.FoodId,
                    FoodName = f.FoodName,
                    Price = f.Price,
                    ImageUrl = f.ImagePath != null ? baseUrl + f.ImagePath : null
                })
                .ToListAsync();
        }

        public async Task<FoodResponseDTO> GetById(int id, string baseUrl)
        {
            var f = await _context.Foods.FindAsync(id);
            if (f == null) throw new Exception("Food not found");

            return new FoodResponseDTO
            {
                FoodId = f.FoodId,
                FoodName = f.FoodName,
                Price = f.Price,
                ImageUrl = f.ImagePath != null ? baseUrl + f.ImagePath : null
            };
        }

        public async Task Create(CreateFoodDTO dto, string imagePath)
        {
            var food = new Food
            {
                FoodName = dto.FoodName,
                Price = dto.Price,
                ImagePath = imagePath
            };

            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, CreateFoodDTO dto, string imagePath)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null) throw new Exception("Food not found");

            food.FoodName = dto.FoodName;
            food.Price = dto.Price;

            if (imagePath != null)
                food.ImagePath = imagePath;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null) throw new Exception("Food not found");

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
        }
    }
}
