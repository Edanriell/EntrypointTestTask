namespace Server.Application.Users.GetClients;

public sealed class GetClientsResponse
{
    public IReadOnlyList<Client> Clients { get; init; } = [];
}

public sealed class Client
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateTime CreatedOnUtc { get; init; }
}
