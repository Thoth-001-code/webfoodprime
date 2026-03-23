namespace webfoodprime.DTOs.Food
{
    public class UpdateFoodDTO
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
    }
}
