namespace Orders.DTOs.RequestDTOs
{
    public class UpdateOrderItemRequest
    {
        public int Quantity { get; set; }
        public decimal? Discount { get; set; }
        public string SpecialInstructions { get; set; }
        public string UpdatedBy { get; set; }
    }
}
