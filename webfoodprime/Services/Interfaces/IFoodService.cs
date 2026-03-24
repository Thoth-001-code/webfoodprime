using webfoodprime.DTOs.Food;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IFoodService
    {
        Task<List<FoodResponseDTO>> GetAll(string baseUrl);
        Task<FoodResponseDTO> GetById(int id, string baseUrl);
        Task Create(CreateFoodDTO dto, string imagePath);
        Task Update(int id, CreateFoodDTO dto, string imagePath);
        Task Delete(int id);
    }
}
