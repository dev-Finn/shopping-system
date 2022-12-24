using Ardalis.ApiEndpoints;
using Catalog.Service.Application.Dtos;
using Catalog.Service.Application.Features;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Service.Features.Products;

public sealed record GetProductRequest(Guid ProductId);

public sealed class GetProductRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetProductRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .NotEmpty();
    }
}

public sealed class GetProductEndpoint : EndpointBaseAsync.WithRequest<GetProductRequest>.WithoutResult
{
    private readonly ILogger<GetProductEndpoint> _logger;
    private readonly ISender _sender;
    private readonly IValidator<GetProductRequest> _validator;

    public GetProductEndpoint(ILogger<GetProductEndpoint> logger, ISender sender, IValidator<GetProductRequest> validator)
    {
        _logger = logger;
        _sender = sender;
        _validator = validator;
    }

    [HttpGet("/products/{ProductId:guid}")]
    [SwaggerOperation(
        Summary = "Get a Product",
        Description = "Get a Product",
        OperationId = "Product.Get",
        Tags = new[] {"Products"})]
    [Produces(typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public override async Task HandleAsync([FromRoute] GetProductRequest request, CancellationToken ct = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            await Results.BadRequest(validationResult.ToDictionary()).ExecuteAsync(HttpContext);
            return;
        }

        ProductDto dto = await _sender.Send(new GetProductQuery(request.ProductId), ct);

        await Results.Ok(dto).ExecuteAsync(HttpContext);
    }
}