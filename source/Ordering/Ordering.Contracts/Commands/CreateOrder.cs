using Ordering.Contracts.Models;

namespace Ordering.Contracts.Commands;

public sealed record SubmitOrder(IReadOnlyCollection<OrderItem> Positions);
