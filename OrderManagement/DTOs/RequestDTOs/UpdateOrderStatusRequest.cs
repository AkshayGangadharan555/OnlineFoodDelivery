namespace Orders.DTOs.RequestDTOs
{
    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; }
        public string UpdatedBy { get; set; }
    }
}
