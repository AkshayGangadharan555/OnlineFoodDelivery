using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenuItemService.Models
{
    public class FoodItem
    {
        [Key]
        public string ItemId { get; set; } // Generate unique ID

        [Required]
        public int RestaurantId { get; set; } // Link to the Restaurant

        [Required]
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
