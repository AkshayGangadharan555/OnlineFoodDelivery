namespace Orders.DTOs.RequestDTOs
{
    public class CreateOrderRequest
    {
        public Guid RestaurantId { get; set; }
        public Guid DeliveryAddressId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; }
    }

}
