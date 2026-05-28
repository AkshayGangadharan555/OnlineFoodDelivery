namespace CartService.Models
{
    public class CartItemDTO
    {
        public int CartItemID { get; set; } 

        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}
