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

    public GetProductQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(GetProductQuery query, CancellationToken ct)
    {
        Product product = await _repository.GetProduct(query.ProductId, ct);
        return Product.AsDto(product);
    }
}