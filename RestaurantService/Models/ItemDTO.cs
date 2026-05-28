// Path: RestaurantService/Models/ItemDto.cs
namespace RestaurantService.Models
{
    public class ItemDto
    {
        public string RestaurantId { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string DishType { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}