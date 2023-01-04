using MassTransit;

namespace Ordering.Contracts.Commands;

[EntityName("cancel-order")]
public sealed record CancelOrder(Guid OrderId);
