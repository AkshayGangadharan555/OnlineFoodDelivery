namespace Orders.DTOs.Request
{
    public class AddToCartRequestDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}
