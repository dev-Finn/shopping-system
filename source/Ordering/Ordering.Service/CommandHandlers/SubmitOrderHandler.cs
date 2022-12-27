using MassTransit;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Ordering.Contracts.Models;

namespace Ordering.Service.CommandHandlers;

public sealed class SubmitOrderHandler : IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        await context.Publish(new OrderSubmitted(new Order(NewId.NextGuid(), context.Message.Positions)), context.CancellationToken);
    }
}
