using Catalog.Service.Domain.Repositories;
using Catalog.Service.Exceptions;

namespace Catalog.Service.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ServiceContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    public IProductRepository ProductRepository { get; }

    public UnitOfWork(ServiceContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
        ProductRepository = new ProductRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }


    public async Task CommitChanges(CancellationToken ct)
    {
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit changes");
            throw new CommitFailedException("Failed to commit changes", ex);
        }
    }
}