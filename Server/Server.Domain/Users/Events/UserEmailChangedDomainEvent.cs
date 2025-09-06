using Server.Domain.Abstractions;

namespace Server.Domain.Users.Events;

public sealed record UserEmailChangedDomainEvent(Guid UserId, Email OldEmail, Email NewEmail) : IDomainEvent;
 
 
