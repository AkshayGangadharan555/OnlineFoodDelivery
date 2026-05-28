using System.ComponentModel.DataAnnotations;

namespace MenuItemsService.Models
{
    public class Orders
    {
        [Key]
         public string? OrderId { get; set; }
        public string RestaurantId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }

    }
}
