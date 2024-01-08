using Microsoft.EntityFrameworkCore;
namespace ShippingParser;

public class MyDbContext : DbContext
{
    public DbSet<Box> Boxes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ShippingParserDatabase.db");
    }
}
