using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Models
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid CustomerId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid RestaurantId { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string CreatedBy { get; set; }

        public ICollection<CartItem> Items { get; set; }
    }
}
