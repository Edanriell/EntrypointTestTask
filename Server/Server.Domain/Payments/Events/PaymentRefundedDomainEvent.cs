using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Shared;

namespace Server.Domain.Payments.Events;

public sealed record
    PaymentRefundedDomainEvent(Guid Id, Guid OrderId, Money Amount, RefundReason Reason) : IDomainEvent;
 
