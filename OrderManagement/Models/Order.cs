using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } 

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
        public DateTime OrderDate { get; set; }

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
        public DateTime CreatedAt { get; set; } 

        [Column(TypeName = "dateTime2")]
        public DateTime UpdatedAt { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string CreatedBy { get; set; }

        // Concurrency token
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<OrderItems> Items { get; set; }
    }

}

