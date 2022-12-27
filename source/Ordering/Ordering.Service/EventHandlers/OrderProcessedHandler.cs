using MassTransit;
using Ordering.Contracts.Events;

namespace Ordering.Service.EventHandlers;

public sealed class OrderProcessedHandler : IConsumer<OrderProcessed>
{
    public async Task Consume(ConsumeContext<OrderProcessed> context)
    {
        throw new NotImplementedException();
    }
}
