using Server.Domain.Abstractions;

namespace Server.Domain.Users.Events;

public sealed record UserUpdatedDomainEvent(Guid UserId) : IDomainEvent;
 
