using Server.Domain.Abstractions;

namespace Server.Domain.Payments.Events;

public sealed record PaymentFailedDomainEvent(Guid Id, Guid OrderId, string reason) : IDomainEvent;
