namespace RestaurantService.Helpers
{
    public class TokenMessage
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
