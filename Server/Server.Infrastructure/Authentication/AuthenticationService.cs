using System.Net.Http.Json;
using Server.Application.Abstractions.Authentication;
using Server.Domain.Users;
using Server.Infrastructure.Authentication.Models;

namespace Server.Infrastructure.Authentication;

// TODO 
// Needs more testing probably we will have problems here
internal sealed class AuthenticationService : IAuthenticationService
{
    private const string PasswordCredentialType = "password";

    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient) { _httpClient = httpClient; }

    public async Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        var userRepresentationModel = UserRepresentationModel.FromUser(
            user
        );

        userRepresentationModel.Credentials = new CredentialRepresentationModel[]
        {
            new()
            {
                Value = password,
                Temporary = false,
                Type = PasswordCredentialType
            }
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "users",
            userRepresentationModel,
            cancellationToken
        );

        return ExtractIdentityIdFromLocationHeader(
            response
        );
    }

    public async Task DeleteUserAsync(string identityId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(
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
        var userRepresentationModel = new UserRepresentationModel
        {
            Email = email,
            Username = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = true
        };

        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(
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
            throw new InvalidOperationException(
                "Location header can't be null"
            );
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
}
