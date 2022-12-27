using MassTransit;

namespace Payment.Contracts.Events;

[EntityName("payment-processed")]
public sealed record PaymentProcessed(Guid OrderId);
