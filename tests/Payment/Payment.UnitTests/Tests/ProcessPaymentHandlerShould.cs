using MassTransit;
using Ordering.Contracts.Commands;
using Payment.Contracts.Events;

namespace Payment.UnitTests.Tests;

public sealed class ProcessPaymentHandlerShould : PaymentTestHarness
{
    [Test]
    public async Task Consume_ProcessPayment_Command()
    {
        await TestHarness.Bus.Publish(new ProcessPayment(NewId.NextGuid(), TestData.GetValidOrderItems(5)));
        Assert.That(await TestHarness.Consumed.Any<ProcessPayment>(), Is.True);
    }

    [Test]
    public async Task Publish_PaymentProcessed_Event()
    {
        await TestHarness.Bus.Publish(new ProcessPayment(NewId.NextGuid(), TestData.GetValidOrderItems(5)));
        Assert.That(await TestHarness.Published.Any<PaymentProcessed>(), Is.True);
    }
}
