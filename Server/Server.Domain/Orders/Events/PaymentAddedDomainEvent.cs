using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Orders.Events;

public sealed record PaymentAddedDomainEvent(
    Guid OrderId,
    Guid PaymentId,
    Money Amount) : IDomainEvent;
