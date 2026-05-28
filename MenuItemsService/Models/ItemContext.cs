using Microsoft.EntityFrameworkCore;

namespace MenuItemsService.Models
{
    public class ItemContext:DbContext
    {
        public ItemContext(DbContextOptions<ItemContext> options) : base(options)
        {
        }
        public DbSet<Item>Items { get; set; }
        public DbSet<Orders> Orders { get; set; }
    }

}
