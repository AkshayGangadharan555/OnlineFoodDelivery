using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Models
{
    public class Orders
    {
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid CustomerId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]

        public Guid RestaurantId { get; set; }
        [Column(TypeName = "uniqueidentifier")]
        public Guid? DeliveryManId { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid PaymentAddressId { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid DeliveryAddressId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ExpectedDeliveryTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ActualDeliveryTime { get; set; }
        
        [Required]
        [Column(TypeName = "dateTime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "dateTime2")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}

