using MassTransit;
using Ordering.Components.StateMachines;
using Ordering.Contracts.Events;
using Ordering.Contracts.Models;
using Ordering.UnitTests.Contracts;
using Shared.NUnit;

namespace Ordering.UnitTests;

public abstract class OrderStateMachineTestHarness : StateMachineTestHarness<OrderStateMachine, OrderState>
{
}
