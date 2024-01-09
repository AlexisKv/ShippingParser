using Microsoft.EntityFrameworkCore;
namespace ShippingParser;

public class MyDbContext : DbContext
{
    public DbSet<Box> Boxes { get; set; }
    
    //in constructor create check for existing of db. - context.Database.EnsureCreated();

    public MyDbContext()
    {
        this.Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ShippingParserDatabase.db");
    }
}
