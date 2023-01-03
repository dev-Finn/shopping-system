using Ordering.Contracts.Commands;

namespace Ordering.UnitTests.Tests;

public sealed class OrderingStateMachineShould : OrderingStateMachineTestHarness
{
    [Test]
    public async Task Create_a_state_instance_when_OrderSubmitted_is_published()
    {
        var validEvent = GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(validEvent);
        Assert.That(SagaHarness.Created.Select(x => x.CorrelationId == validEvent.OrderId).Any, Is.True);
    }

    [Test]
    public async Task Publish_an_ReserveStock_Command_when_OrderSubmitted_is_processed()
    {
        var validEvent = GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(validEvent);
        Assert.That(await TestHarness.Published.Any<ReserveStock>(), Is.True);
    }
}
