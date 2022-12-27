using Ardalis.ApiEndpoints;
using Catalog.Service.Application.Features;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Service.Endpoints.Products;

public sealed record UpdateProductRequestBody(string Name, string Description, decimal Price);

public sealed record UpdateProductRequest([FromRoute] Guid ProductId, [FromBody] UpdateProductRequestBody Body);

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .NotEmpty();
        
        RuleFor(request => request.Body.Name)
            .NotNull()
            .NotEmpty()
            .Length(5, 64);
        
        RuleFor(request => request.Body.Description)
            .NotNull()
            .NotEmpty()
            .Length(5, 512);
        
        RuleFor(request => request.Body.Price)
            .GreaterThan(0);
    }
}

public sealed class UpdateProductEndpoint : EndpointBaseAsync.WithRequest<UpdateProductRequest>.WithActionResult
{
    private readonly ISender _sender;
    private readonly IValidator<UpdateProductRequest> _validator;

    public UpdateProductEndpoint(ISender sender, IValidator<UpdateProductRequest> validator)
    {
        _sender = sender;
        _validator = validator;
    }

    [HttpPut("/products/{ProductId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Updates a Product",
        Description = "Updates a Product",
        OperationId = "Product.Update",
        Tags = new[] {"Products"})]
    public override async Task<ActionResult> HandleAsync([FromRoute] UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        await _sender.Send(new UpdateProductCommand(request.ProductId, request.Body.Name, request.Body.Description, request.Body.Price), cancellationToken);

        return Ok();
    }
}