using Ardalis.ApiEndpoints;
using Catalog.Service.Application.Features;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Service.Features.Products;

public sealed record CreateProductRequest(string Name, string Description , decimal Price);

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
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

public sealed class CreateProductEndpoint : EndpointBaseAsync.WithRequest<CreateProductRequest>.WithActionResult
{
    private readonly ISender _sender;
    private readonly ILogger<CreateProductEndpoint> _logger;
    private readonly IValidator<CreateProductRequest> _validator;

    public CreateProductEndpoint(ISender sender, ILogger<CreateProductEndpoint> logger, IValidator<CreateProductRequest> validator)
    {
        _sender = sender;
        _logger = logger;
        _validator = validator;
    }
    
    [HttpPost("/products")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Creates a Product",
        Description = "Creates a Product",
        OperationId = "Product.Create",
        Tags = new[] {"Products"})]
    public override async Task<ActionResult> HandleAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        Guid id =await _sender.Send(new CreateProductCommand(request.Name, request.Description, request.Price),
            cancellationToken);

        return Created("", id);
    }
}