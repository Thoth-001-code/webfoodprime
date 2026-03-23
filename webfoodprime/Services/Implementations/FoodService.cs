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

        public async Task<List<Food>> GetAll()
        {
            return await _context.Foods.ToListAsync();
        }

        public async Task<Food> GetById(int id)
        {
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
                throw new Exception("Food not found");

            return food;
        }

        public async Task Create(CreateFoodDTO dto)
        {
            var food = new Food
            {
                FoodName = dto.FoodName,
                Price = dto.Price,
                ImagePath = dto.ImagePath
            };

            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
        }

        public async Task Update(UpdateFoodDTO dto)
        {
            var food = await _context.Foods.FindAsync(dto.FoodId);

            if (food == null)
                throw new Exception("Food not found");

            food.FoodName = dto.FoodName;
            food.Price = dto.Price;
            food.ImagePath = dto.ImagePath;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
                throw new Exception("Food not found");

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
        }
    }
}
