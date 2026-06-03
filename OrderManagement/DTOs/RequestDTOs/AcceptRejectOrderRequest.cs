namespace Orders.DTOs.RequestDTOs
{
    public class AcceptRejectOrderRequest
    {
        public bool Accepted { get; set; }
        public Guid DeliveryAgentId { get; set; }
    }

}
