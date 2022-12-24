using Ardalis.ApiEndpoints;
using Catalog.Service.Application.Dtos;
using Catalog.Service.Application.Features;
using Catalog.Service.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Service.Features.Products;

public sealed class GetPaginatedProductsResponse : PagedResponse<ProductDto>
{
    public GetPaginatedProductsResponse(PaginatedList<ProductDto> data) : base(data)
    {
    }   
}

public sealed class GetPaginatedProductsEndpoint : EndpointBaseAsync.WithRequest<SieveModel>.WithResult<GetPaginatedProductsResponse>
{
    private readonly ILogger<GetPaginatedProductsEndpoint> _logger;
    private readonly ISender _sender;

    public GetPaginatedProductsEndpoint(ISender sender, ILogger<GetPaginatedProductsEndpoint> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpGet("/products")]
    [SwaggerOperation(
        Summary = "Get Paginated Products",
        Description = "Get Paginated Products",
        OperationId = "Product.PaginatedList",
        Tags = new[] {"Products"})]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public override async Task<GetPaginatedProductsResponse> HandleAsync(SieveModel sieveModel, CancellationToken ct = default)
    {
        PaginatedList<ProductDto> paginatedList = await _sender.Send(new GetPaginatedProductsQuery(sieveModel), ct);

        return new GetPaginatedProductsResponse(paginatedList);
    }
}