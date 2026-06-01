namespace Orders.DTOs.RequestDTOs
{
    public class ValidateProductRequest
    {
        public Guid ProductId { get; set; }
        public Guid RestaurantId { get; set; }
    }
}
