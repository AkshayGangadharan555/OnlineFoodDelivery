namespace Orders.DTOs.Request
{
    public class AssignDeliveryRequestDto
    {
        public Guid OrderId { get; set; }
        public Guid DeliveryManId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
