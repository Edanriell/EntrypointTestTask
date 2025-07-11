using Server.Domain.Abstractions;

namespace Server.Domain.Payments.Events;

public sealed record PaymentExpiredDomainEvent(Guid PaymentId, Guid OrderId) : IDomainEvent;
