using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Payments.Events;

public sealed record PartialPaymentReceivedDomainEvent(
    Guid OrderId,
    Guid ClientId,
    Money PaymentAmount,
    Money OutstandingAmount) : IDomainEvent;
 
