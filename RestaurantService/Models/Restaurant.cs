
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantService.Models
{

public class Restaurant
{
    [Key]
    
    public string? RestaurantId { get; set; }

    [Required]
    public string RestaurantName { get; set; }
    public string Category { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MaxLength(10)]
    [RegularExpression(@"^[0-9]{10}$")]
    public string PhoneNumber { get; set; }

    public string Address { get; set; }
    public string City { get; set; }
    public string BranchName { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
    public string FssaiLicenseNumber { get; set; }
    public string VerificationStatus { get; set; }  // pending ,rejected, verified.
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string PasswordHash { get; set; }
}
}

