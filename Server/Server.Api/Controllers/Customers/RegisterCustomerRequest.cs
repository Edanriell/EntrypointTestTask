using Server.Domain.Users;

namespace Server.Api.Controllers.Customers;
 
public sealed record RegisterCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Gender Gender,
    string Country,
    string City,
    string ZipCode,
    string Street,
    string Password);
