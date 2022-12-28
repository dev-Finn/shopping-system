using Catalog.Service.Application.Contracts;
using Catalog.Service.Domain.Models;
using Catalog.Service.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Sieve.Models;
using Sieve.Services;

namespace Catalog.Service.Application.Features;

public sealed record GetPaginatedProductsQuery(SieveModel SieveModel)
    : IRequest<PaginatedList<ProductListItem>>;

public sealed class GetPaginatedProductsQueryHandler : IRequestHandler<GetPaginatedProductsQuery, PaginatedList<ProductListItem>>
{
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IProductRepository _repository;

    public GetPaginatedProductsQueryHandler(ISieveProcessor sieveProcessor, IProductRepository repository)
    {
        _sieveProcessor = sieveProcessor;
        _repository = repository;
    }

    public async Task<PaginatedList<ProductListItem>> Handle(GetPaginatedProductsQuery query, CancellationToken ct)
    {
        IQueryable<Product> queryable = _repository.Products.AsNoTracking();
        IQueryable<Product>? totalCountQueryable = _sieveProcessor.Apply(query.SieveModel, queryable, applyPagination: false);
        int totalCount = await totalCountQueryable.CountAsync(ct);
        IQueryable<Product>? paginatedQueryable = _sieveProcessor.Apply(query.SieveModel, queryable);
        List<Product> list = await paginatedQueryable.ToListAsync(ct);
        List<ProductListItem> listDto = list.Select(Product.AsListItem).ToList();
        return new PaginatedList<ProductListItem>(listDto, totalCount, query.SieveModel.Page!.Value,
            query.SieveModel.PageSize.Value);
    }
}