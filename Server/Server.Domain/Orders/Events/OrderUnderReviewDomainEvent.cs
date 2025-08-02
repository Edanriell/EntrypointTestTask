using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderUnderReviewDomainEvent(
    Guid OrderId,
    string Reason) : IDomainEvent;
