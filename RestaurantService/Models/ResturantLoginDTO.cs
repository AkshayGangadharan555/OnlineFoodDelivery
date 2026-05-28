using System.ComponentModel.DataAnnotations;

namespace RestaurantService.Models
{
    public class RestaurantLoginDTO
    {


        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
    }
}
