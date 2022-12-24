using MassTransit;
using Ordering.Service.Contracts.Commands.Incomming;
using Ordering.Service.Contracts.Commands.Outgoing;
using Ordering.Service.Contracts.Events.Incomming;
using Ordering.Service.Contracts.Events.Outgoing;
using Ordering.Service.Models;

namespace Ordering.Service.Sagas;

public sealed class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public IReadOnlyCollection<Position> Positions { get; set; } = new List<Position>();
}

public sealed class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => Create, correlation => correlation.SelectId(context => context.CorrelationId ?? NewId.NextGuid()));
        Event(() => Pay, correlation => correlation.CorrelateById(context => context.Message.OrderId));
        Event(() => Ship, correlation => correlation.CorrelateById(context => context.Message.OrderId));
        Event(() => Cancel, correlation => correlation.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(Create)
                .Then(x => x.Saga.Positions = x.Message.Positions)
                .PublishAsync(context =>
                    context.Init<OrderCreatedEvent>(new { OrderId = context.Saga.CorrelationId, context.Saga.Positions, TimestampUtc = DateTime.UtcNow }))
                .TransitionTo(Pending));

        During(Pending,
            When(Pay)
                .SendAsync(context => context.Init<ShipOrderCommand>(new { OrderId = context.Saga.CorrelationId }))
                .TransitionTo(Paid));

        During(Paid, When(Ship).TransitionTo(Shipped).Finalize());

        DuringAny(
            When(Cancel)
                .PublishAsync(context => context.Init<OrderCancelledEvent>(new { OrderId = context.Saga.CorrelationId, TimestampUtc = DateTime.UtcNow }))
                .TransitionTo(Cancelled)
                .Finalize());
    }

    public Event<CreateOrderCommand> Create { get; private set; }
    public Event<CancelOrderCommand> Cancel { get; private set; }
    public Event<OrderPaidEvent> Pay { get; private set; }
    public Event<OrderShippedEvent> Ship { get; private set; }

    public State Pending { get; private set; }
    public State Paid { get; private set; }
    public State Shipped { get; private set; }
    public State Cancelled { get; private set; }
}
