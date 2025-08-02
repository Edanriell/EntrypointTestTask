using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Products.Events;

public sealed record ProductReservationCancelledDomainEvent(Guid ProductId, Quantity CancelledQuantity) : IDomainEvent;
