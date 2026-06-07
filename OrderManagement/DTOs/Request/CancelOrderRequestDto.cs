namespace Orders.DTOs.Request
{
    public class CancelOrderRequestDto
    {
        public Guid OrderId { get; set; }

        public string? CancelReason { get; set; }
    }
}
