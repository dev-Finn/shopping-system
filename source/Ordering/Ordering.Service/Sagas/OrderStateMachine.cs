using MassTransit;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Ordering.Contracts.Models;
using Payment.Contracts.Events;
using Warehouse.Contracts.Events;

namespace Ordering.Service.Sagas;

public sealed class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Order Order { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public List<string> History { get; set; } = new();
}

public sealed class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        SetupCorrelation();

        Initially(SetOrderSubmittedHandler());

        During(Processing, SetStockReservedHandler(), SetPaymentProcessedHandler(), SetOrderShippedHandler());

        During(Processing, SetOrderCancelledHandler());
    }

    private void SetupCorrelation()
    {
        Event(() => OrderSubmitted, correlation => correlation.CorrelateById(context => context.Message.Order.OrderId));
        Event(() => StockReserved, correlation => correlation.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentProcessed, correlation => correlation.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderShipped, correlation => correlation.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCanceled, correlation => correlation.CorrelateById(context => context.Message.OrderId));
    }

    private EventActivityBinder<OrderState, OrderSubmitted> SetOrderSubmittedHandler() =>
        When(OrderSubmitted)
            .Then(x => x.Saga.Created = DateTime.UtcNow)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Then(x => x.Saga.Order = x.Message.Order)
            .Then(x => x.Saga.History.Add(x.Event.Name))
            .Publish(c => new ReserveStock(c.Saga.Order))
            .TransitionTo(Processing);

    private EventActivityBinder<OrderState, StockReserved> SetStockReservedHandler() =>
        When(StockReserved)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Then(x => x.Saga.History.Add(x.Event.Name))
            .Publish(c => new ProcessPayment(c.Saga.Order));

    private EventActivityBinder<OrderState, PaymentProcessed> SetPaymentProcessedHandler() =>
        When(PaymentProcessed)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Then(x => x.Saga.History.Add(x.Event.Name))
            .Publish(c => new ShipOrder(c.Saga.Order));

    private EventActivityBinder<OrderState, OrderShipped> SetOrderShippedHandler() =>
        When(OrderShipped)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Then(x => x.Saga.History.Add(x.Event.Name))
            .Publish(c => new OrderProcessed(c.Saga.Order.OrderId))
            .TransitionTo(Processed)
            .Finalize();

    private EventActivityBinder<OrderState, OrderCancelled> SetOrderCancelledHandler() =>
        When(OrderCanceled)
            .Then(x => x.Saga.Updated = DateTime.UtcNow)
            .Then(x => x.Saga.History.Add(x.Event.Name))
            .Publish(c => new OrderCancelled(c.Saga.Order.OrderId))
            .TransitionTo(Cancelled)
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
