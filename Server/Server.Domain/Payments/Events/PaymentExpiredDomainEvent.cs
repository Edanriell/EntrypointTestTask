using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Payments.Events;

public sealed record PaymentExpiredDomainEvent(Guid PaymentId, Guid OrderId, Money Amount) : IDomainEvent;
 
