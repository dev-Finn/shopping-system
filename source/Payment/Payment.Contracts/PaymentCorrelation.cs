using System.Runtime.CompilerServices;
using MassTransit;
using Payment.Contracts.Events;

namespace Payment.Contracts;

public static class PaymentCorrelation
{
    [ModuleInitializer]
    public static void Initialize()
    {
        LogContext.Info?.Log("Initializing Payment Message Correlation...");

        MessageCorrelation.UseCorrelationId<PaymentProcessed>(x => x.OrderId);

        LogContext.Info?.Log("Payment Message Correlation Initialized!");
    }
}
