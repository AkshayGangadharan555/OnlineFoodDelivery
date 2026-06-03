namespace Orders.DTOs.Response
{
    public class OrderItemResponseDto
    {
        public Guid OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal Discount { get; set; }

        public decimal SubTotal { get; set; }

        public string? SpecialInstructions { get; set; }

        public string Status { get; set; }
    }
}