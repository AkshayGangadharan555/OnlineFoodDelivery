using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenuItemsService.Models
{
    public class Item
    {
        [Key]
        public string? ItemId { get; set; }  



        public string RestaurantId { get; set; }


        public string ItemName { get; set; }
        public string Category { get; set; }
        public string DishType { get; set; }
        public string Description { get; set; }

       
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
