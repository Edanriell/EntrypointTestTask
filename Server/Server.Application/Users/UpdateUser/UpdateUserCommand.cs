using Server.Application.Abstractions.Messaging;
using Server.Domain.Users;

namespace Server.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    Gender? Gender,
    string? Country,
    string? City,
    string? ZipCode,
    string? Street) : ICommand;
 
