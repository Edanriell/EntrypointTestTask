using Server.Domain.Abstractions;

namespace Server.Domain.Payments.Events;

public sealed record PaymentProcessingStartedDomainEvent(Guid PaymentId, Guid OrderId) : IDomainEvent;
