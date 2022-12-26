using MassTransit;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Ordering.Contracts.Models;

namespace Ordering.Service.Sagas;

public sealed class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Order Order { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}

public sealed class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        SetupCorrelation();

        Initially(SetOrderSubmittedHandler());

        During(Processing, SetStockReservedHandler(), SetPaymentProcessedHandler(), SetOrderShippedHandler());

        During(Processing, When(OrderCanceled).TransitionTo(Cancelled).Finalize());

        SetCompletedWhenFinalized();
    }

    private void SetupCorrelation()
    {
        Event(() => OrderSubmitted, correlation => correlation.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentProcessed, correlation => correlation.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderShipped, correlation => correlation.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCanceled, correlation => correlation.CorrelateById(context => context.Message.OrderId));
    }


    private EventActivityBinder<OrderState, OrderSubmitted> SetOrderSubmittedHandler() =>
        When(OrderSubmitted)
            .Then(x => x.Saga.Created = DateTime.UtcNow)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Send(c => new ReserveStock(c.Saga.Order))
            .TransitionTo(Processing);

    private EventActivityBinder<OrderState, StockReserved> SetStockReservedHandler() =>
        When(StockReserved)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Then(c => new ProcessPayment(c.Saga.Order));

    private EventActivityBinder<OrderState, PaymentProcessed> SetPaymentProcessedHandler() =>
        When(PaymentProcessed)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Send(c => new ShipOrder(c.Saga.Order));

    private EventActivityBinder<OrderState, OrderShipped> SetOrderShippedHandler() =>
        When(OrderShipped)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Publish(c => new OrderProcessed(c.Saga.Order.OrderId))
            .TransitionTo(Processed)
            .Finalize();

    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<OrderCancelled> OrderCanceled { get; private set; }
    public Event<StockReserved> StockReserved { get; private set; }
    public Event<PaymentProcessed> PaymentProcessed { get; private set; }
    public Event<OrderShipped> OrderShipped { get; private set; }

    public State Processing { get; private set; }
    public State Processed { get; private set; }
    public State Cancelled { get; private set; }
}
