using System.Diagnostics;
using Catalog.Service.Application.Dtos;
using Catalog.Service.Domain.Models;
using Catalog.Service.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Sieve.Models;
using Sieve.Services;

namespace Catalog.Service.Application.Features;

public sealed record GetPaginatedProductsQuery(SieveModel SieveModel)
    : IRequest<PaginatedList<ProductDto>>;

public sealed class GetPaginatedProductsQueryHandler : IRequestHandler<GetPaginatedProductsQuery, PaginatedList<ProductDto>>
{
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IProductRepository _repository;
    private readonly ILogger<GetPaginatedProductsQueryHandler> _logger;

    public GetPaginatedProductsQueryHandler(ISieveProcessor sieveProcessor, IProductRepository repository, ILogger<GetPaginatedProductsQueryHandler> logger)
    {
        _sieveProcessor = sieveProcessor;
        _repository = repository;
        _logger = logger;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetPaginatedProductsQuery query, CancellationToken ct)
    {
        IQueryable<Product> queryable = _repository.Products.AsNoTracking();
        IQueryable<Product>? totalCountQueryable = _sieveProcessor.Apply(query.SieveModel, queryable, applyPagination: false);
        int totalCount = await totalCountQueryable.CountAsync(ct);
        IQueryable<Product>? paginatedQueryable = _sieveProcessor.Apply(query.SieveModel, queryable);
        List<Product> list = await paginatedQueryable.ToListAsync(ct);
        List<ProductDto> listDto = list.Select(Product.AsDto).ToList();
        Debug.Assert(query.SieveModel.PageSize != null, "query.SieveModel.PageSize != null");
        return new PaginatedList<ProductDto>(listDto, totalCount, query.SieveModel.Page!.Value,
            query.SieveModel.PageSize.Value);
    }
}