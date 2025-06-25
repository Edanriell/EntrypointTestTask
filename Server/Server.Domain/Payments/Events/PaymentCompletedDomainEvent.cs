using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Payments.Events;

public sealed record PaymentCompletedDomainEvent(
    Guid OrderId,
    Guid ClientId,
    Money PaidAmount) : IDomainEvent;
