using Server.Domain.Abstractions;

namespace Server.Domain.Users.Events;

public sealed record UserDeletedDomainEvent(Guid UserId, string Email) : IDomainEvent;
