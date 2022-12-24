using Catalog.Service.Domain.Repositories;
using MediatR;

namespace Catalog.Service.Application.Features;

public sealed record DeleteProductCommand(Guid ProductId) : IRequest<Unit>;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        await _unitOfWork.ProductRepository.DeleteProduct(command.ProductId, ct);
        await _unitOfWork.CommitChanges(ct);
        return Unit.Value;
    }
}