using Microsoft.EntityFrameworkCore;

namespace TestTaskParsing.Domain
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Employee> Employees { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
