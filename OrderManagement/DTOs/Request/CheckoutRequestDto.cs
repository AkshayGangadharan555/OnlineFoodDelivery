namespace Orders.DTOs.Request
{
    public class CheckoutRequestDto
    {
        public Guid RestaurantId { get; set; }
        public Guid PaymentAddressId { get; set; }
        public Guid DeliveryAddressId { get; set; }
    }
}
