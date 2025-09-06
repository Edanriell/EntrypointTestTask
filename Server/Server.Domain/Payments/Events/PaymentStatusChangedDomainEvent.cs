using Server.Domain.Abstractions;

namespace Server.Domain.Payments.Events;

public sealed record PaymentStatusChangedDomainEvent(
    Guid OrderId,
    Guid PaymentId,
    PaymentStatus OldStatus,
    PaymentStatus NewStatus) : IDomainEvent;
 
