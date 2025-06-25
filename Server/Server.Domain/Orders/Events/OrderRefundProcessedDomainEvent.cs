using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Orders.Events;

public sealed record OrderRefundProcessedDomainEvent(
    Guid OrderId,
    Guid ClientId,
    Money RefundAmount,
    RefundReason Reason) : IDomainEvent;
