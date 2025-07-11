namespace Server.Application.Users.GetClients;

public sealed record GetUsersResponse(
    IReadOnlyList<User> Users
);

public sealed class User
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Gender { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public DateTime CreatedOnUtc { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = new List<string>();
    public IReadOnlyList<string> Permissions { get; init; } = new List<string>();
}
