namespace webfoodprime.DTOs.Admin
{
    public class TopFoodDTO
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public int TotalSold { get; set; }
        public string ImageUrl { get; set; } // 🔥 thêm
        public decimal Price { get; set; }   // 🔥 thêm
    }
}
