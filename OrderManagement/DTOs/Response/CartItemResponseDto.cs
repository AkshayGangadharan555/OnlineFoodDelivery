namespace Orders.DTOs.Response
{
    public class CartItemResponseDto
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
        public string? SpecialInstructions { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
