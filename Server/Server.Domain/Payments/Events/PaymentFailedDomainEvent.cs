using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Payments.Events;

public sealed record PaymentFailedDomainEvent(
    Guid Id,
    Guid OrderId,
    Money Amount,
    PaymentFailureReason PaymentFailureReason) : IDomainEvent;
 
