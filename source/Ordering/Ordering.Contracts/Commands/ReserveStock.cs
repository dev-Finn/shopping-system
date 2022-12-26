using Ordering.Contracts.Models;

namespace Ordering.Contracts.Commands;

public sealed record ReserveStock(Order Order);
