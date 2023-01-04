using Ordering.Contracts.Commands;

namespace Ordering.UnitTests.Tests;

public sealed class OrderStateMachineShould : OrderStateMachineTestHarness
{
    [Test]
    public async Task Create_State_Instance_when_OrderSubmitted_is_published()
    {
        var validEvent = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(validEvent);
        Assert.That(SagaHarness.Created.Select(x => x.CorrelationId == validEvent.OrderId).Any, Is.True);
    }

    [Test]
    public async Task Publish_ReserveStock_when_OrderSubmitted_is_processed()
    {
        var validEvent = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(validEvent);
        Assert.That(await TestHarness.Published.Any<ReserveStock>(), Is.True);
    }
}
