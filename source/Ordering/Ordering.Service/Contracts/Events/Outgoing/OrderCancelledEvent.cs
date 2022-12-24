namespace Ordering.Service.Contracts.Events.Outgoing;

public sealed record OrderCancelledEvent(Guid OrderId, DateTime TimestampUtc);
