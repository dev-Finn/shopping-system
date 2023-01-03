using MassTransit;
using Payment.Components.Consumers;
using Shared.NUnit;

namespace Payment.UnitTests;

public abstract class PaymentTestHarness : MassTransitTestHarness
{
    protected override void ConfigureTestHarness(IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<ProcessPaymentHandler>();
    }
}
