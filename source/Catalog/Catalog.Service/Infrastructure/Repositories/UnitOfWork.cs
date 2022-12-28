using System.Diagnostics;
using Catalog.Service.Domain.Repositories;
using Catalog.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Service.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CatalogContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    
    public IProductRepository ProductRepository { get; }

    public UnitOfWork(CatalogContext context, ILogger<UnitOfWork> logger)
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

            Exception specificException = ex switch
            {
                OperationCanceledException operationCanceledException => operationCanceledException,
                DbUpdateConcurrencyException dbUpdateConcurrencyException => new PleaseRetryAgainException("Failed to commit changes", ex),
                DbUpdateException dbUpdateException => new CommitFailedException("Failed to commit changes", ex),
                _ => throw new UnreachableException()
            };
            throw specificException;
        }
    }
}