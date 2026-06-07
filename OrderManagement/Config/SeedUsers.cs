using Orders.Models;

namespace Orders.Config
{
    public static class SeedUsers
    {
        public static List<User> Get()
        {
            return new List<User>
            {
                new()
                {
                    Id = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"),
                    Username = "",
                    Password = "customer123",
                    Role = "Customer",
                    Name = "Pavan"
                },
                new()
                {
                    Id = Guid.Parse("B2C3D4E5-F6A7-8901-BCDE-F12345678901"),
                    Username = "restaurant1",
                    Password = "restaurant123",
                    Role = "Restaurant",
                    Name = "Pizza Hut"
                },
                new()
                {
                    Id = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"),
                    Username = "agent1",
                    Password = "agent123",
                    Role = "DeliveryAgent",
                    Name = "Dude Delivery"
                }
            };
        }
    }
}
