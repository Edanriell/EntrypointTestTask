using Server.Application.Abstractions.Messaging;
using Server.Domain.Users;

namespace Server.Application.Users.RegisterCustomer;
 
public sealed record RegisterCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Gender Gender,
    string Country,
    string City,
    string ZipCode,
    string Street,
    string Password) : ICommand<Guid>;
