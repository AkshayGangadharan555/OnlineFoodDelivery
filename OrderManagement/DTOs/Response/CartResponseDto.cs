namespace Orders.DTOs.Response
{
    public class CartResponseDto
    {
        public Guid CartId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new();
    }
}
