using System;

namespace RestaurantService.Models
{
    public class OrderDto
    {
        
        public string ItemName { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
}