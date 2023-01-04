using Ordering.Components.Consumers;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;

namespace Ordering.UnitTests.Tests;

public sealed class SubmitOrderHandlerShould : OrderingTestHarness
{
    [Test]
    public async Task Consume_SubmitOrder_Command()
    {
        await TestHarness.Bus.Publish(TestData.GetValidSubmitOrderCommand());
        Assert.That(await TestHarness.GetConsumerHarness<SubmitOrderHandler>().Consumed.Any<SubmitOrder>(), Is.True);
    }

    [Test]
    public async Task Publish_OrderSubmitted_Event()
    {
        await TestHarness.Bus.Publish(TestData.GetValidSubmitOrderCommand());
        Assert.That(await TestHarness.Published.Any<OrderSubmitted>(), Is.True);
    }
}
