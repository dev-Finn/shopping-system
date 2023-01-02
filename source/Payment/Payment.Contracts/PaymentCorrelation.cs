using System.Runtime.CompilerServices;
using MassTransit;
using Payment.Contracts.Events;

namespace Payment.Contracts;

public static class PaymentCorrelation
{
    [ModuleInitializer]
    public static void Initialize()
    {
        MessageCorrelation.UseCorrelationId<PaymentProcessed>(x => x.OrderId);
    }
}
