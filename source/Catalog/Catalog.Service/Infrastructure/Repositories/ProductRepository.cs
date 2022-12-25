using Catalog.Service.Domain.Models;
using Catalog.Service.Domain.Repositories;
using Catalog.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Service.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly ServiceContext _context;
    public DbSet<Product> Products { get; }

    public ProductRepository(ServiceContext context)
    {
        _context = context;
        Products = context.Set<Product>();
    }
    
    public async Task<Guid> CreateProduct(string name, string description, decimal price, CancellationToken ct = default)
    {
        Product product = Product.Create(name, description, price);

        await _context.Products.AddAsync(product, ct);

        return product.Id;
    }

    public async Task DeleteProduct(Guid productId, CancellationToken ct = default)
    {
        Product product = await FindProduct(productId, ct);

        _context.Products.Remove(product);
    }

    public async Task UpdateProduct(Guid productId, string name, string description, decimal price, CancellationToken ct = default)
    {
        Product product = await FindProduct(productId, ct);
        
        product.Update(name, description, price);

        _context.Products.Update(product);
    }

    public Task<Product> GetProduct(Guid productId, CancellationToken ct = default) => FindProduct(productId, ct);

    private async Task<Product> FindProduct(Guid productId, CancellationToken ct)
    {
        Product? product = await _context.Products.FindAsync(new object?[] { productId }, ct);

        if (product is null)
        {
            throw new ProductNotFoundException($"Product with Id: {productId} not found!");
        }

        return product;
    }
     
}