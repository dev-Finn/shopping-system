using Catalog.Service.Application.Contracts;

namespace Catalog.Service.Domain.Models;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    
    public DateTime? DeletedAt { get; private set; }

    public Product()
    {
        
    }

    private Product(Guid id, string name, string description, decimal price)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
    }
    
    public static Product Create(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length is < 5 or > 64)
        {
            throw new ArgumentException("Name can not be null or whitespace.", nameof(name));
        }
        
        if (string.IsNullOrWhiteSpace(description) || description.Length is < 5 or > 512)
        {
            throw new ArgumentException("Description can not be null or whitespace.", nameof(description));
        }
        
        if (price <= 0)
        {
            throw new ArgumentException("Price can not be smaller or equal 0.",nameof(price));
        }
        
        return new Product(Guid.NewGuid(), name, description, price);
    }

    public void Update(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name can not be null or whitespace.", nameof(name));
        }
        
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description can not be null or whitespace.", nameof(description));
        }
        
        if (price <= 0)
        {
            throw new ArgumentException("Price can not be smaller or equal 0.",nameof(price));
        }
        
        Name = name;
        Description = description;
        Price = price;
    }

    public void Delete()
    {
        if (DeletedAt is not null)
        {
            return;
        }
        
        DeletedAt = DateTime.UtcNow;
    }

    public static ProductListItem AsListItem(Product product)
        => new(product.Id, product.Name, product.Description, product.Price, product.DeletedAt);
    
    public static ProductDetail AsDto(Product product)
        => new(product.Id, product.Name, product.Description, product.Price, product.DeletedAt);
}