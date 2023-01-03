using MassTransit;
using Ordering.Components.StateMachines;
using Ordering.Contracts.Events;
using Ordering.Contracts.Models;
using Ordering.UnitTests.Contracts;
using Shared.NUnit;

namespace Ordering.UnitTests;

public abstract class OrderStateMachineTestHarness : StateMachineTestHarness<OrderStateMachine, OrderState>
{
    protected static OrderSubmitted GetValidOrderSubmittedEvent()
    {
        return new OrderSubmitted(NewId.NextGuid(), GetValidOrderItems(Random.Shared.Next(10)));
    }

    protected static IReadOnlyCollection<IOrderItem> GetValidOrderItems(int amount)
    {
        return Enumerable.Range(0, amount).Select(x => new OrderItem(
            Guid.NewGuid(),
            Random.Shared.Next(150),
            Random.Shared.Next(150))).ToList();
    }
}
