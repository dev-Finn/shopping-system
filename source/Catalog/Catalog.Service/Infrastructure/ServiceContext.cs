using Catalog.Service.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Service.Infrastructure;

public sealed class CatalogContext : DbContext
{
    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}