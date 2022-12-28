using Ardalis.ApiEndpoints;
using Catalog.Service.Application.Features;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Service.Endpoints.Products;

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
    private readonly IValidator<DeleteProductRequest> _validator;


    public DeleteProductEndpoint(ISender sender, IValidator<DeleteProductRequest> validator)
    {
        _sender = sender;
        _validator = validator;
    }

    [HttpDelete("/products/{ProductId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Deletes a Product",
        Description = "Deletes a Product",
        OperationId = "Product.Delete",
        Tags = new[] {"Products"})]
    public override async Task<ActionResult> HandleAsync([FromRoute] DeleteProductRequest request, CancellationToken cancellationToken = new CancellationToken())
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