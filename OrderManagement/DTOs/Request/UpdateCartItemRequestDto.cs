namespace Orders.DTOs.Request
{
    public class UpdateCartItemRequestDto
    {
        public int Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}
