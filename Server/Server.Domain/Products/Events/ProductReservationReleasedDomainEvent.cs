using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Products.Events;

public sealed record ProductReservationReleasedDomainEvent(Guid ProductId, Quantity ReleasedQuantity) : IDomainEvent;
 
