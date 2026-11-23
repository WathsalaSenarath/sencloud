using Microsoft.EntityFrameworkCore;
using SenCloud.Api.Models;

namespace SenCloud.Api.Data;

public class SenCloudDbContext(DbContextOptions<SenCloudDbContext> options) : DbContext(options)
{
    public DbSet<App> Apps => Set<App>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<App>(e =>
        {
            e.HasIndex(a => a.Name).IsUnique();          // no two apps with the same name/URL
            e.Property(a => a.Name).HasMaxLength(63);    // Kubernetes name limit
            e.Property(a => a.Image).HasMaxLength(255);
            e.Property(a => a.Status).HasConversion<string>().HasMaxLength(20); // store "Running", not 2
        });
    }
}