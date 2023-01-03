using MassTransit;
using Ordering.Contracts.Commands;
using Payment.Contracts.Events;
using Payment.UnitTests.Contracts;

namespace Payment.UnitTests.Tests;

public sealed class ProcessPaymentConsumerShould : PaymentTestHarness
{
    [Test]
    public async Task Consume_ProcessPayment_Command()
    {
        await TestHarness.Bus.Publish(new ProcessPayment(NewId.NextGuid(), TestData.GetValidOrderItems(5)));
        Assert.That(await TestHarness.Consumed.Any<ProcessPayment>(), Is.True);
    }

    [Test]
    public async Task Publishes_PaymentProcessed_Event()
    {
        await TestHarness.Bus.Publish(new ProcessPayment(NewId.NextGuid(), TestData.GetValidOrderItems(5)));
        Assert.That(await TestHarness.Published.Any<PaymentProcessed>(), Is.True);
    }
}
