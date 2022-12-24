using Ordering.Service.Models;

namespace Ordering.Service.Contracts.Events.Outgoing;

public sealed record OrderCreatedEvent(Guid OrderId, IReadOnlyCollection<Position> Positions, DateTime TimestampUtc);
