using MassTransit;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;

namespace Ordering.Service.CommandHandlers;

public sealed class CancelOrderHandler : IConsumer<CancelOrder>
{
    public async Task Consume(ConsumeContext<CancelOrder> context)
    {
        await context.Publish(new OrderCancelled(context.Message.OrderId), context.CancellationToken);
    }
}
