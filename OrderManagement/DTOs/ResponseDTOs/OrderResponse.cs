namespace Orders.DTOs.ResponseDTOs
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid DeliveryAddressId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemResponse> Items { get; set; }
    }

}
