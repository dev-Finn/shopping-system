namespace Ordering.Service.Contracts.Events.Incomming;

public sealed record OrderShippedEvent(Guid OrderId);
