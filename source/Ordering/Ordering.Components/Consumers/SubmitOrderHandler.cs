using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;

namespace Ordering.Components.Consumers;

public sealed class SubmitOrderHandler : IConsumer<SubmitOrder>
{
    private readonly ILogger<SubmitOrderHandler> _logger;

    public SubmitOrderHandler(ILogger<SubmitOrderHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        _logger.LogInformation("Received Order submission with {ItemCount} Items", context.Message.Items.Count);
        await context.Publish(new OrderSubmitted(NewId.NextGuid(), context.Message.Items), context.CancellationToken);
    }
}
