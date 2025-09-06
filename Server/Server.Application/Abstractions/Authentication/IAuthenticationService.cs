using Server.Domain.Users;

namespace Server.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);

    Task<string> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        string identityId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(
        string identityId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task AdminResetPasswordAsync(
        string identityId,
        string newPassword,
        bool isTemporary = false,
        CancellationToken cancellationToken = default);

    // Mirrors fields in KeyCloak
    Task UpdateUserAsync(
        string identityId,
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task DeleteUserAsync(string identityId, CancellationToken cancellationToken = default);
}
