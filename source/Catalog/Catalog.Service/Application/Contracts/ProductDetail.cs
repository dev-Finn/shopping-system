namespace Catalog.Service.Application.Contracts;

public sealed record ProductDetail(Guid Id, string Name, string Description, decimal Price, DateTime? DeletedAt);