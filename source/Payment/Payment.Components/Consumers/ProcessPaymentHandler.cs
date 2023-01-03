using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Contracts.Commands;
using Payment.Contracts.Events;

namespace Payment.Components.Consumers;

public sealed class ProcessPaymentHandler : IConsumer<ProcessPayment>
{
    private readonly ILogger<ProcessPaymentHandler> _logger;

    public ProcessPaymentHandler(ILogger<ProcessPaymentHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        _logger.LogInformation("Received Payment processing request for Order {OrderId}", context.Message.OrderId);
        await context.Publish(new PaymentProcessed(context.Message.OrderId), context.CancellationToken);
    }
}
