using Catalog.Service.Domain.Models;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Catalog.Service.Infrastructure.SieveProcessors;

public sealed class ProductSieveProcessor : SieveProcessor
{
    public ProductSieveProcessor(IOptions<SieveOptions> options) : base(options) { }

    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        mapper.Property<Product>(p => p.Name)
            .CanSort()
            .CanFilter();

        mapper.Property<Product>(p => p.Description)
            .CanSort()
            .CanFilter();
        
        mapper.Property<Product>(p => p.Price)
            .CanSort()
            .CanFilter();
        
        mapper.Property<Product>(p => p.DeletedAt)
            .CanSort()
            .CanFilter();
        
        return base.MapProperties(mapper);
    }
}