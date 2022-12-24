using Catalog.Service.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Service.Infrastructure;

public sealed class ServiceContext : DbContext
{
    public ServiceContext(DbContextOptions<ServiceContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}