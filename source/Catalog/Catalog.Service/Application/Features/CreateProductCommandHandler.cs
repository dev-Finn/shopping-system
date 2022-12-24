using Catalog.Service.Domain.Repositories;
using MediatR;

namespace Catalog.Service.Application.Features;

public sealed record CreateProductCommand(string Name, string Description, decimal Price) : IRequest<Guid>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Guid> Handle(CreateProductCommand command, CancellationToken ct)
    {
        Guid id = await _unitOfWork.ProductRepository.CreateProduct(command.Name, command.Description, command.Price, ct);
        await _unitOfWork.CommitChanges(ct);
        return id;
    }
}