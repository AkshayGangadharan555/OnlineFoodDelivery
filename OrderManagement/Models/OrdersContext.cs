using Microsoft.EntityFrameworkCore;

namespace Orders.Models
{
    public class OrdersContext : DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Status constraints
            modelBuilder.Entity<Order>()
                .ToTable(t => t.HasCheckConstraint("CK_Orders_Status",
                    "[Status] IN ('Pending','Confirmed','Preparing','Ready','Assigned','PickUp','Delivered','Cancelled')"));

            modelBuilder.Entity<OrderItems>()
                .ToTable(t => t.HasCheckConstraint("CK_OrderItem_Status",
                    "[Status] IN ('Pending','Preparing','Ready','Cancelled')"));

            // Relationships (cascade delete)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Essential indexes only
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerId);   // fetch orders by customer
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);       // filter by order status

            modelBuilder.Entity<OrderItems>()
                .HasIndex(i => i.OrderId);      // link items to orders
            modelBuilder.Entity<OrderItems>()
                .HasIndex(i => i.Status);       // filter items by status
        }
    }


}
