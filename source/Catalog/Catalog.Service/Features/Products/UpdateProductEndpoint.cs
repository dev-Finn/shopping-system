using Ardalis.ApiEndpoints;
using Catalog.Service.Application.Features;
using Catalog.Service.Helpers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Service.Features.Products;

public sealed record UpdateProductRequest([FromRoute] Guid ProductId, [FromBody] string Name, [FromBody] string Description, [FromBody] decimal Price);

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .NotEmpty();
        
        RuleFor(request => request.Name)
            .NotNull()
            .NotEmpty()
            .Length(5, 64);
        
        RuleFor(request => request.Description)
            .NotNull()
            .NotEmpty()
            .Length(5, 512);
        
        RuleFor(request => request.Price)
            .GreaterThan(0);
    }
}

public sealed class UpdateProductEndpoint : EndpointBaseAsync.WithRequest<UpdateProductRequest>.WithActionResult
{
    private readonly ISender _sender;
    private readonly ILogger<UpdateProductEndpoint> _logger;
    private readonly IValidator<UpdateProductRequest> _validator;

    public UpdateProductEndpoint(ISender sender, ILogger<UpdateProductEndpoint> logger, IValidator<UpdateProductRequest> validator)
    {
        _sender = sender;
        _logger = logger;
        _validator = validator;
    }

    [HttpPut("/products/{ProductId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Updates a Product",
        Description = "Updates a Product",
        OperationId = "Product.Update",
        Tags = new[] {"Products"})]
    public override async Task<ActionResult> HandleAsync([FromMultiSource] UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        await _sender.Send(new UpdateProductCommand(request.ProductId, request.Name, request.Description, request.Price), cancellationToken);

        return Ok();
    }
}