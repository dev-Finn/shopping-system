using Ardalis.ApiEndpoints;
using Catalog.Service.Application.Features;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Service.Features.Products;

public sealed record DeleteProductRequest(Guid ProductId);

public sealed class DeleteProductRequestValidator : AbstractValidator<DeleteProductRequest>
{
    public DeleteProductRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .NotEmpty();
    }
}

public sealed class DeleteProductEndpoint : EndpointBaseAsync.WithRequest<DeleteProductRequest>.WithActionResult
{
    private readonly ISender _sender;
    private readonly ILogger<DeleteProductEndpoint> _logger;
    private readonly IValidator<DeleteProductRequest> _validator;


    public DeleteProductEndpoint(ISender sender, ILogger<DeleteProductEndpoint> logger, IValidator<DeleteProductRequest> validator)
    {
        _sender = sender;
        _logger = logger;
        _validator = validator;
    }

    [HttpDelete("/products")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Deletes a Product",
        Description = "Deletes a Product",
        OperationId = "Product.Delete",
        Tags = new[] {"Products"})]
    public override async Task<ActionResult> HandleAsync(DeleteProductRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        await _sender.Send(new DeleteProductCommand(request.ProductId), cancellationToken);

        return Ok();
    }
}