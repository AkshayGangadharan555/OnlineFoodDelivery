namespace Orders.DTOs.RequestDTOs
{
    public class CreateOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public Guid RestaurantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TaxAmount { get; set; }
        public string ItemDescription { get; set; }
        public string SpecialInstructions { get; set; }
    }
}
