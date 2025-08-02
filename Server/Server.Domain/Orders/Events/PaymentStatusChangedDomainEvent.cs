using Server.Domain.Abstractions;
using Server.Domain.Payments;

namespace Server.Domain.Orders.Events;

public sealed record PaymentStatusChangedDomainEvent(
    Guid OrderId,
    Guid PaymentId,
    PaymentStatus OldStatus,
    PaymentStatus NewStatus
) : IDomainEvent;
