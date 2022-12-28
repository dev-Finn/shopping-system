namespace Catalog.Service.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IProductRepository ProductRepository { get; }

    Task CommitChanges(CancellationToken ct);
}