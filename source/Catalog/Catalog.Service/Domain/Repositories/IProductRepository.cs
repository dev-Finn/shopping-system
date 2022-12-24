using Catalog.Service.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Service.Domain.Repositories;

public interface IProductRepository
{
    Task<Guid> CreateProduct(string name, string description, decimal price, CancellationToken ct = default);
    Task DeleteProduct(Guid productId, CancellationToken ct = default);
    Task UpdateProduct(Guid productId, string name, string description, decimal price, CancellationToken ct = default);
    Task<Product> GetProduct(Guid productId, CancellationToken ct = default);
    DbSet<Product> Products { get; }
}