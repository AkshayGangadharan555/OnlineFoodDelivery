namespace Orders.DTOs.RequestDTOs
{
    public class BulkCreateOrderItemsRequest
    {
        public Guid OrderId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
