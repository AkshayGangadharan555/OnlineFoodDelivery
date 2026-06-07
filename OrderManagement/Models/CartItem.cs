using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Models
{
    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid CartId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid RestaurantId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ProductId { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? ProductName { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? SpecialInstructions { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        public Cart Cart { get; set; }
    }
}
