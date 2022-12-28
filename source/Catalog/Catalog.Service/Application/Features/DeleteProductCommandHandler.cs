using Catalog.Service.Domain.Repositories;
using MediatR;

namespace Catalog.Service.Application.Features;

public sealed record DeleteProductCommand(Guid ProductId) : IRequest<Unit>;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        await _unitOfWork.ProductRepository.DeleteProduct(command.ProductId, ct);
        await _unitOfWork.CommitChanges(ct);
        return Unit.Value;
    }
}