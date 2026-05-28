using Microsoft.EntityFrameworkCore;

namespace CartService.Models
{
    public class CartItemsContext : DbContext
    {
        public CartItemsContext(DbContextOptions<CartItemsContext> options)
            : base(options)
        {
        }

        public DbSet<CartItems> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItems>().ToTable("CartItems");
        }
    }
}
