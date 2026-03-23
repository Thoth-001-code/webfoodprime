using webfoodprime.DTOs.Food;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IFoodService
    {
        Task<List<Food>> GetAll();
        Task<Food> GetById(int id);
        Task Create(CreateFoodDTO dto);
        Task Update(UpdateFoodDTO dto);
        Task Delete(int id);
    }
}
