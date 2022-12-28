namespace Catalog.Service.Application.Contracts;

public sealed record ProductListItem(Guid Id, string Name, string Description, decimal Price, DateTime? DeletedAt);