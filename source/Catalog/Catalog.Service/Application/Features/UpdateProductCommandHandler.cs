using Catalog.Service.Domain.Repositories;
using MediatR;

namespace Catalog.Service.Application.Features;

public sealed record UpdateProductCommand
    (Guid ProductId, string Name, string Description, decimal Price)
    : IRequest<Unit>;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        await _unitOfWork.ProductRepository.UpdateProduct(command.ProductId, command.Name, command.Description, command.Price, ct);
        await _unitOfWork.CommitChanges(ct);
        return Unit.Value;
    }
}