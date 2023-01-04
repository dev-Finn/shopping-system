using MassTransit;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Ordering.Contracts.Models;
using Ordering.Contracts.Responses;
using Payment.Contracts.Events;
using Warehouse.Contracts.Events;

namespace Ordering.Components.StateMachines;

public sealed class OrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public IReadOnlyCollection<IOrderItem> Items { get; set; }

    public int Version { get; set; }
    public Guid? ReservationTimeoutTokenId { get; set; }
}

public sealed record OrderReservationTimeoutExpired(Guid OrderId);

public sealed class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCancellationRequested, e =>
        {
            e.OnMissingInstance(m =>
                m.ExecuteAsync(x => x.RespondAsync(new OrderNotFound(x.Message.OrderId))));
        });

        Schedule(() => ReservationTimeout, instance => instance.ReservationTimeoutTokenId, s =>
        {
            s.Delay = TimeSpan.FromDays(6);
            s.Received = r => r.CorrelateById(context => context.Message.OrderId);
        });

        Initially(When(OrderSubmitted).InitializeState().TransitionTo(Submitted).ReserveStock());

        During(Submitted,
            When(StockReserved)
                .TransitionTo(Reserved)
                .Schedule(ReservationTimeout, context => new OrderReservationTimeoutExpired(context.Saga.CorrelationId))
                .InitiatePaymentProcessing());

        During(Reserved,
            When(PaymentProcessed)
                .Unschedule(ReservationTimeout)
                .TransitionTo(Paid)
                .ShipOrder());

        During(Reserved, When(ReservationTimeout.Received)
            .CancelOrder()
            .TransitionTo(ReservationTimedOut).Finalize());

        During(Paid, When(OrderShipped).TransitionTo(Shipped).CompleteOrder().Finalize());

        DuringAny(When(OrderCancellationRequested).CancelOrder().TransitionTo(Cancelled).Finalize());
    }

    public Event<OrderSubmitted> OrderSubmitted { get; }
    public Event<CancelOrder> OrderCancellationRequested { get; }
    public Event<StockReserved> StockReserved { get; }
    public Event<PaymentProcessed> PaymentProcessed { get; }
    public Event<OrderShipped> OrderShipped { get; }

    public Schedule<OrderState, OrderReservationTimeoutExpired> ReservationTimeout { get; }

    public State Submitted { get; }
    public State Reserved { get; }
    public State ReservationTimedOut { get; }
    public State Paid { get; }
    public State Shipped { get; }
    public State Cancelled { get; }
}

file static class OrderStateMachineBehaviorExtensions
{
    public static EventActivityBinder<OrderState, OrderSubmitted> InitializeState(
        this EventActivityBinder<OrderState, OrderSubmitted> binder) =>
        binder
            .Then(x => x.Saga.Items = x.Message.Items)
            .Then(x => LogContext.Info?.Log("Order {0} with {1} Items submitted", x.Saga.CorrelationId, x.Saga.Items.Count));

    public static EventActivityBinder<OrderState, OrderSubmitted> ReserveStock(
        this EventActivityBinder<OrderState, OrderSubmitted> binder) =>
        binder
            .Publish(c => new ReserveStock(c.Saga.CorrelationId, c.Saga.Items));

    public static EventActivityBinder<OrderState, StockReserved> InitiatePaymentProcessing(
        this EventActivityBinder<OrderState, StockReserved> binder) =>
        binder
            .Then(x => LogContext.Info?.Log("Stock Reserved for Order {0}", x.Saga.CorrelationId))
            .Publish(c => new ProcessPayment(c.Saga.CorrelationId, c.Saga.Items));

    public static EventActivityBinder<OrderState, PaymentProcessed> ShipOrder(
        this EventActivityBinder<OrderState, PaymentProcessed> binder) =>
        binder
            .Then(x => LogContext.Info?.Log("Payment Processed for Order {0}", x.Saga.CorrelationId))
            .Publish(c => new ShipOrder(c.Saga.CorrelationId, c.Saga.Items));

    public static EventActivityBinder<OrderState, OrderShipped> CompleteOrder(
        this EventActivityBinder<OrderState, OrderShipped> binder) =>
        binder
            .Then(x => LogContext.Info?.Log("Order {0} sucessfully shipped", x.Saga.CorrelationId))
            .Publish(c => new OrderProcessed(c.Saga.CorrelationId));

    public static EventActivityBinder<OrderState, CancelOrder> CancelOrder(
        this EventActivityBinder<OrderState, CancelOrder> binder) =>
        binder
            .Then(x => LogContext.Info?.Log("Order {0} was cancelled", x.Saga.CorrelationId))
            .Publish(c => new OrderCancelled(c.Saga.CorrelationId));

    public static EventActivityBinder<OrderState, OrderReservationTimeoutExpired> CancelOrder(
        this EventActivityBinder<OrderState, OrderReservationTimeoutExpired> binder) =>
        binder
            .Then(x => LogContext.Info?.Log("Order {0} was cancelled due to the reservation timing out", x.Saga.CorrelationId))
            .Publish(c => new OrderCancelled(c.Saga.CorrelationId));
}
