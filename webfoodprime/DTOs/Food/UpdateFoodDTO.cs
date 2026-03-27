namespace webfoodprime.DTOs.Food
{
    public class UpdateFoodDTO
    {
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; } // Có thể null nếu không đổi ảnh
    }
}
