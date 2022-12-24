namespace Ordering.Service.Contracts.Events.Incomming;

public sealed record OrderPaidEvent(Guid OrderId);
