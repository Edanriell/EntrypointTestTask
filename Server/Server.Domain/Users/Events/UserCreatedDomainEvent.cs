using Server.Domain.Abstractions;

namespace Server.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(Guid Id) : IDomainEvent;
 
