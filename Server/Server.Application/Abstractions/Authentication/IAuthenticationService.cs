using Server.Domain.Users;

namespace Server.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);

    Task UpdateUserAsync(
        string identityId,
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task DeleteUserAsync(string identityId, CancellationToken cancellationToken = default);
}
