using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Payments.Events;

public sealed record PaymentProcessedDomainEvent(Guid Id, Guid OrderId, Money Amount) : IDomainEvent;
 
