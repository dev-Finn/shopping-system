using MassTransit;
using Ordering.Contracts.Commands;
using Payment.Contracts.Events;

namespace Payment.Components.Consumers;

public sealed class ProcessPaymentHandler : IConsumer<ProcessPayment>
{
    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        await context.Publish(new PaymentProcessed(context.Message.OrderId), context.CancellationToken);
    }
}
