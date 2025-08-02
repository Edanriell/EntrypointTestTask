using Server.Domain.Abstractions;

namespace Server.Domain.Payments.Events;

public sealed record PaymentDisputedDomainEvent(Guid PaymentId, Guid OrderId, string DisputeReason) : IDomainEvent;
 
