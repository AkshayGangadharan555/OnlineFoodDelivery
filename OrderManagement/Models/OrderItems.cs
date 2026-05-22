using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Models
{
    public class OrderItems
    {
        [Key]
        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid OrderItemId { get; set; } = Guid.NewGuid();

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid OrderId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ProductId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid RestuarantId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TaxAmount { get; set; }
        public string? ItemDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal => (Quantity * UnitPrice) - (Discount ?? 0) + (TaxAmount ?? 0);

        [Column(TypeName = "nvarchar(500)")]
        public string? SpecialInstructions { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string? Status { get; set; }

        [Required]
        [Column(TypeName = "dateTime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "dateTime2")]
        public DateTime LastUpdatedAt { get; set;} = DateTime.UtcNow;
    }
}
