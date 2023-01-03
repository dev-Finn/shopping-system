using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Contracts.Commands;
using Warehouse.Contracts.Events;

namespace Warehouse.Components.Consumers;

public sealed class ReserveStockHandler : IConsumer<ReserveStock>
{
    private readonly ILogger<ReserveStockHandler> _logger;

    public ReserveStockHandler(ILogger<ReserveStockHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReserveStock> context)
    {
        _logger.LogInformation("Processing item reservation request for Order {OrderId}", context.Message.OrderId);
        await context.Publish(new StockReserved(context.Message.OrderId), context.CancellationToken);
    }
}
