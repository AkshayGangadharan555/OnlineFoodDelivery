namespace RestaurantService.Models
{
    public class RestaurantUpdateDTO
    {
        public string RestaurantName { get; set; }
        public string Category { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string BranchName { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
