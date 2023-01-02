using MassTransit;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;

namespace Ordering.Components.Consumers;

public sealed class SubmitOrderHandler : IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        await context.Publish(new OrderSubmitted(NewId.NextGuid(), context.Message.Positions), context.CancellationToken);
    }
}
