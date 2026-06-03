namespace Orders.DTOs.Request
{
    public class PlaceOrderRequestDto
    {
        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid PaymentAddressId { get; set; }
        public Guid DeliveryAddressId { get; set; }
        public List<OrderItemRequestDto> Items { get; set; } = new();
    }
}
