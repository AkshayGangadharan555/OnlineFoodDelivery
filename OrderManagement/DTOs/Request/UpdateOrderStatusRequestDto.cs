namespace Orders.DTOs.Request
{
    public class UpdateOrderStatusRequestDto
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public string? StatusRemarks { get; set; }
        public string? Remarks { get => StatusRemarks; set => StatusRemarks = value; }
        public byte[] RowVersion { get; set; }
    }
}
