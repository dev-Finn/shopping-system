namespace Catalog.Service.Application.Dtos;

public sealed record ProductDto(Guid Id, string Name, string Description, decimal Price, DateTime? DeleteAt);