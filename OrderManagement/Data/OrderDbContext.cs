using Microsoft.EntityFrameworkCore;
using Orders.Constants;
using Orders.Models;

namespace Orders.Data
{
    public class OrderDbContext: DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options): base(options){}
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Order>()
                .Property(o =>o.RowVersion).IsRowVersion();

            modelBuilder
                .Entity<Order>()
                .ToTable(t =>
                    t.HasCheckConstraint(
                    "CK_Order_Status",
                    "Status IN (" +
                    "'Pending'," +
                    "'Confirmed'," +
                    "'Preparing'," +
                    "'Ready'," +
                    "'Assigned'," +
                    "'PickedUp'," +
                    "'Delivered'," +
                    "'Cancelled')"));

            modelBuilder
                .Entity<OrderItems>()
                .ToTable(t =>
                    t.HasCheckConstraint(
                    "CK_OrderItem_Status",
                    "Status IN (" +
                    "'Pending'," +
                    "'Preparing'," +
                    "'Ready'," +
                    "'Cancelled')"));

            modelBuilder
                .Entity<Order>()
                .HasIndex(o => o.CustomerId);

            modelBuilder
                .Entity<Order>()
                .HasIndex(o => o.RestaurantId);

            modelBuilder
                .Entity<Order>()
                .HasIndex(o => o.DeliveryManId);

            modelBuilder
                .Entity<Order>()
                .HasIndex(o => o.Status);

            modelBuilder
                .Entity<Order>()
                .Property(o => o.Status).HasDefaultValue(OrderStatuses.Pending);

            modelBuilder
                .Entity<OrderItems>()
                .Property(oi => oi.Status).HasDefaultValue(OrderItemStatuses.Pending);

            modelBuilder
                .Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Cart>()
                .HasIndex(c => c.CustomerId);

            modelBuilder
                .Entity<Cart>()
                .HasIndex(c => c.RestaurantId);
        }
    }
}