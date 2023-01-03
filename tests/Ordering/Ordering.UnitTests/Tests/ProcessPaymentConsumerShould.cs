using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Ordering.UnitTests.Contracts;

namespace Ordering.UnitTests.Tests;

public sealed class ProcessPaymentConsumerShould : OrderingTestHarness
{
    [Test]
    public async Task Consume_SubmitOrder_Command()
    {
        await TestHarness.Bus.Publish(new SubmitOrder(new[] { new OrderItem(Guid.NewGuid(), 15, 10) }));
        Assert.That(await TestHarness.Consumed.Any<SubmitOrder>(), Is.True);
    }

    [Test]
    public async Task Publishes_OrderSubmitted_Event()
    {
        await TestHarness.Bus.Publish(new SubmitOrder(new[] { new OrderItem(Guid.NewGuid(), 15, 10) }));
        Assert.That(await TestHarness.Published.Any<OrderSubmitted>(), Is.True);
    }
}
