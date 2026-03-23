namespace webfoodprime.DTOs.Food
{
    public class CreateFoodDTO
    {
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
    }
}
