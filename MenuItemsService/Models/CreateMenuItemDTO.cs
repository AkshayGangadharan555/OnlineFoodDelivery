namespace MenuItemsService.Models
{
    public class CreateMenuItemDTO
    {
        public string ItemName { get; set; }
                                      
        public string DishType { get; set; }  // Breakfast, Main Course, etc.
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
