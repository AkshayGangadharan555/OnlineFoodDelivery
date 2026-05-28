using Microsoft.EntityFrameworkCore;

namespace RestaurantService.Models
{
    public class RestaurantContext:DbContext
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options)
        {
        }
        public DbSet<Restaurant>Restaurants { get; set; }
    }
}
