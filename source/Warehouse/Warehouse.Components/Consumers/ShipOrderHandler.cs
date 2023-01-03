using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Contracts.Commands;
using Warehouse.Contracts.Events;

namespace Warehouse.Components.Consumers;

public sealed class ShipOrderHandler : IConsumer<ShipOrder>
{
    private readonly ILogger<ShipOrderHandler> _logger;

    public ShipOrderHandler(ILogger<ShipOrderHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ShipOrder> context)
    {
        _logger.LogInformation("Received shipping request for Order {OrderId}", context.Message.OrderId);
        await context.Publish(new OrderShipped(context.Message.OrderId), context.CancellationToken);
    }
}