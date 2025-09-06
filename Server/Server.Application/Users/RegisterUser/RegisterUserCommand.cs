using Server.Application.Abstractions.Messaging;
using Server.Domain.Users;

namespace Server.Application.Users.RegisterUser;
 
public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Gender Gender,
    string Country,
    string City,
    string ZipCode,
    string Street,
    string Password,
    IEnumerable<string>? RoleNames = null) : ICommand<Guid>;
  
