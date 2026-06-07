namespace Orders.DTOs.Response
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }
}
