using Server.Domain.Abstractions;

namespace Server.Domain.Users.Events;

public sealed record UserPersonalInfoUpdatedDomainEvent(
    Guid UserId,
    FirstName FirstName,
    LastName LastName) : IDomainEvent;
