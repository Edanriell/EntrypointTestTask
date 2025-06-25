using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Orders.Events;

public sealed record OrderConfirmedDomainEvent(Guid OrderId, Guid ClientId, Money TotalAmount) : IDomainEvent;
