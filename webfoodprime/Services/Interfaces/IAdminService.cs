using webfoodprime.DTOs.Admin;

namespace webfoodprime.Services.Interfaces
{
    public interface IAdminService
    {

        Task<DashboardDTO> GetDashboard();
        Task<IEnumerable<TopFoodDTO>> GetTopFoods(int top = 5);
        
    }
}
