using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Server.Application.Abstractions.Authentication;
using Server.Domain.Users;
using Server.Infrastructure.Authentication.Models;

namespace Server.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private const string PasswordCredentialType = "password";
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;
    private string? _adminToken;
    private DateTime _adminTokenExpiry = DateTime.MinValue;

    public AuthenticationService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    public async Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        var userRepresentationModel = UserRepresentationModel.FromUser(user);

        userRepresentationModel.Credentials = new CredentialRepresentationModel[]
        {
            new()
            {
                Value = password,
                Temporary = false,
                Type = PasswordCredentialType
            }
        };

        using HttpClient adminClient = CreateAdminClient();

        HttpResponseMessage response = await adminClient.PostAsJsonAsync(
            "users",
            userRepresentationModel,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"User registration failed: {errorContent}");
        }

        return ExtractIdentityIdFromLocationHeader(response);
    }

    public async Task<string> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "password"),
            new("username", email),
            new("password", password),
            new("client_id", _keycloakOptions.AuthClientId)
        };

        if (!string.IsNullOrEmpty(_keycloakOptions.AuthClientSecret))
        {
            parameters.Add(new KeyValuePair<string, string>("client_secret", _keycloakOptions.AuthClientSecret));
        }

        var content = new FormUrlEncodedContent(parameters);

        using var tokenClient = new HttpClient();

        HttpResponseMessage response = await tokenClient.PostAsync(
            _keycloakOptions.TokenUrl,
            content,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Access token is null");
    }

    public async Task ChangePasswordAsync(
        string identityId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        await ValidateCurrentPasswordAsync(identityId, currentPassword, cancellationToken);
        await PerformPasswordResetAsync(identityId, newPassword, false, cancellationToken);
    }

    public async Task ResetPasswordAsync(
        string identityId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        await ChangePasswordAsync(identityId, currentPassword, newPassword, cancellationToken);
    }

    public async Task AdminResetPasswordAsync(
        string identityId,
        string newPassword,
        bool isTemporary = false,
        CancellationToken cancellationToken = default)
    {
        await PerformPasswordResetAsync(identityId, newPassword, isTemporary, cancellationToken);
    }

    public async Task DeleteUserAsync(string identityId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        using HttpClient adminClient = CreateAdminClient();

        HttpResponseMessage response = await adminClient.DeleteAsync(
            $"users/{identityId}",
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateUserAsync(
        string identityId,
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        var userRepresentationModel = new UserRepresentationModel
        {
            Email = email,
            Username = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = true
        };

        using HttpClient adminClient = CreateAdminClient();

        HttpResponseMessage response = await adminClient.PutAsJsonAsync(
            $"users/{identityId}",
            userRepresentationModel,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    private static string ExtractIdentityIdFromLocationHeader(HttpResponseMessage httpResponseMessage)
    {
        const string usersSegmentName = "users/";

        string? locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header can't be null");
        }

        int userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase
        );

        string userIdentityId = locationHeader.Substring(
            userSegmentValueIndex + usersSegmentName.Length
        );

        return userIdentityId;
    }

    private async Task PerformPasswordResetAsync(
        string identityId,
        string newPassword,
        bool isTemporary,
        CancellationToken cancellationToken)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        var credentialRepresentation = new CredentialRepresentationModel
        {
            Type = PasswordCredentialType,
            Value = newPassword,
            Temporary = isTemporary
        };

        using HttpClient adminClient = CreateAdminClient();

        HttpResponseMessage response = await adminClient.PutAsJsonAsync(
            $"users/{identityId}/reset-password",
            credentialRepresentation,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            HandleKeycloakError(response, "Password reset");
        }
    }

    private async Task ValidateCurrentPasswordAsync(
        string identityId,
        string currentPassword,
        CancellationToken cancellationToken)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        using HttpClient adminClient = CreateAdminClient();

        HttpResponseMessage userResponse = await adminClient.GetAsync(
            $"users/{identityId}",
            cancellationToken
        );

        if (!userResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("User not found");
        }

        string userJson = await userResponse.Content.ReadAsStringAsync(cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        UserRepresentationModel? userInfo;
        try
        {
            userInfo = JsonSerializer.Deserialize<UserRepresentationModel>(userJson, options);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize user information: {ex.Message}");
        }

        if (userInfo is null)
        {
            throw new InvalidOperationException("Failed to deserialize user information - result is null");
        }

        string loginIdentifier = !string.IsNullOrEmpty(userInfo.Username) ? userInfo.Username : userInfo.Email;

        if (string.IsNullOrEmpty(loginIdentifier))
        {
            throw new InvalidOperationException("User information incomplete - no username or email available");
        }

        try
        {
            await LoginAsync(loginIdentifier, currentPassword, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException("Current password is incorrect");
        }
    }

    private async Task EnsureAdminTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_adminToken) || DateTime.UtcNow >= _adminTokenExpiry)
        {
            await RefreshAdminTokenAsync(cancellationToken);
        }
    }

    private async Task RefreshAdminTokenAsync(CancellationToken cancellationToken)
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
            new("client_id", _keycloakOptions.AdminClientId),
            new("client_secret", _keycloakOptions.AdminClientSecret)
        };

        var content = new FormUrlEncodedContent(parameters);

        using var tokenClient = new HttpClient();

        HttpResponseMessage response = await tokenClient.PostAsync(
            _keycloakOptions.TokenUrl,
            content,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Admin authentication failed: {errorContent}");
        }

        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        _adminToken = tokenResponse?.AccessToken ?? throw new InvalidOperationException("Admin token is null");
        _adminTokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);
    }

    private HttpClient CreateAdminClient()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_keycloakOptions.AdminUrl)
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

        return client;
    }

    private static void HandleKeycloakError(HttpResponseMessage response, string operation)
    {
        string errorContent = response.Content.ReadAsStringAsync().Result;

        throw response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => new UnauthorizedAccessException($"{operation} failed: Unauthorized"),
            HttpStatusCode.NotFound => new InvalidOperationException($"{operation} failed: User not found"),
            HttpStatusCode.BadRequest => new ArgumentException($"{operation} failed: {errorContent}"),
            HttpStatusCode.Conflict => new InvalidOperationException($"{operation} failed: Resource conflict"),
            _ => new InvalidOperationException($"{operation} failed: {errorContent}")
        };
    }
}
