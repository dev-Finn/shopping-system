using Catalog.Service.Domain.Repositories;
using MediatR;

namespace Catalog.Service.Application.Features;

public sealed record CreateProductCommand(string Name, string Description, decimal Price) : IRequest<Guid>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Guid> Handle(CreateProductCommand command, CancellationToken ct)
    {
        Guid id = await _unitOfWork.ProductRepository.CreateProduct(command.Name, command.Description, command.Price, ct);
        await _unitOfWork.CommitChanges(ct);
        return id;
    }
}