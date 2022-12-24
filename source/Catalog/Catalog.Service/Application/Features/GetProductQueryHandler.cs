using Catalog.Service.Application.Dtos;
using Catalog.Service.Domain.Models;
using Catalog.Service.Domain.Repositories;
using MediatR;

namespace Catalog.Service.Application.Features;

public sealed record GetProductQuery(Guid ProductId)
    : IRequest<ProductDto>;

public sealed class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<GetProductQueryHandler> _logger;

    public GetProductQueryHandler(IProductRepository repository, ILogger<GetProductQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(GetProductQuery query, CancellationToken ct)
    {
        Product product = await _repository.GetProduct(query.ProductId, ct);
        return product.AsDto();
    }
}