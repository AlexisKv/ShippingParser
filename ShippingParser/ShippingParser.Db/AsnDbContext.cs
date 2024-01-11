using Microsoft.EntityFrameworkCore;
using ShippingParser.Entities;

namespace ShippingParser.Db;

public class AsnDbContext : DbContext
{
    public DbSet<Box> Boxes { get; set; }

    public AsnDbContext()
    {
        this.Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ShippingParserDatabase.db");
    }
}
