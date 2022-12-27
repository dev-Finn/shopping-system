using Ardalis.ApiEndpoints;
using Catalog.Service.Application.Dtos;
using Catalog.Service.Application.Features;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Sieve.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Service.Endpoints.Products;

public sealed class GetPaginatedProductsResponse : PagedResponse<ProductDto>
{
    public GetPaginatedProductsResponse(PaginatedList<ProductDto> data) : base(data)
    {
    }   
}

public sealed class GetPaginatedProductsEndpoint : EndpointBaseAsync.WithRequest<SieveModel>.WithResult<GetPaginatedProductsResponse>
{
    private readonly ISender _sender;

    public GetPaginatedProductsEndpoint(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("/products")]
    [SwaggerOperation(
        Summary = "Get Paginated Products",
        Description = "Get Paginated Products",
        OperationId = "Product.PaginatedList",
        Tags = new[] {"Products"})]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<GetPaginatedProductsResponse> HandleAsync([FromQuery] SieveModel sieveModel, CancellationToken ct = default)
    {
        PaginatedList<ProductDto> paginatedList = await _sender.Send(new GetPaginatedProductsQuery(sieveModel), ct);

        return new GetPaginatedProductsResponse(paginatedList);
    }
}