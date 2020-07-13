using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Orders.Db
{
    public class OrderDbContext: DbContext
    {
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(DbContextOptions options): base(options)
        {
            
        }
    }
}