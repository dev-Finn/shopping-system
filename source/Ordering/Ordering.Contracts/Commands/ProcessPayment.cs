using Ordering.Contracts.Models;

namespace Ordering.Contracts.Commands;

public sealed record ProcessPayment(Order Order);
