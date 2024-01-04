using Microsoft.EntityFrameworkCore;
namespace ShippingParser;

public class MyDbContext : DbContext
{
    public DbSet<Box> MyEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=mydatabase.db");
    }
}
