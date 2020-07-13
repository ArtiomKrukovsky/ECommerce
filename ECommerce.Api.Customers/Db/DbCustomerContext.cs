using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ECommerce.Api.Customers.Db
{
    public class DbCustomerContext: DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbCustomerContext(DbContextOptions options): base(options)
        {
            
        }
    }
}