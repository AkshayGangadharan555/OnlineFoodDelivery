using Microsoft.EntityFrameworkCore;

namespace Orders.Models
{
    public class OrdersContext(DbContextOptions<OrdersContext> options) : DbContext(options)
    {
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }

        //[Obsolete]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Orders>()
                .ToTable(t => t.HasCheckConstraint("CK_Orders_Status", "[Status] IN ('Pending', 'Confirmed', 'Preparing', 'Ready', 'Assigned', 'PickUp', 'Delivered', 'Cancelled')"));

            modelBuilder.Entity<OrderItems>()
                .ToTable(t => t.HasCheckConstraint("CK_OrderItem_Status", "[status] IN ('Pending', 'Preparing', 'Ready', 'Cancelled')"));
        }
    }
}
