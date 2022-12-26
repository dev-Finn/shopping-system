using Ordering.Contracts.Models;

namespace Ordering.Contracts.Events;

public sealed record StockReserved(Guid OrderId);
