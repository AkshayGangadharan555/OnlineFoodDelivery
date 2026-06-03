namespace Orders.DTOs.Response
{
    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid? DeliveryManId { get; set; }
        public Guid PaymentAddressId { get; set; }
        public Guid DeliveryAddressId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string? StatusRemarks { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
