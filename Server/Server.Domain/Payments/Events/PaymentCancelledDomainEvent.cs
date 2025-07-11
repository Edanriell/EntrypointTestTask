using Server.Domain.Abstractions;

namespace Server.Domain.Payments.Events;

public sealed record PaymentCancelledDomainEvent(Guid PaymentId, Guid OrderId, string Reason) : IDomainEvent;
