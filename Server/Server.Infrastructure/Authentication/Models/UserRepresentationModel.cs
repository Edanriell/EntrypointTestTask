using System.Text.Json.Serialization;
using Server.Domain.Users;

namespace Server.Infrastructure.Authentication.Models;

internal sealed class UserRepresentationModel
{
    [JsonPropertyName("access")] public Dictionary<string, object>? Access { get; set; }

    [JsonPropertyName("attributes")] public Dictionary<string, List<string>>? Attributes { get; set; }

    [JsonPropertyName("clientRoles")] public Dictionary<string, string>? ClientRoles { get; set; }

    [JsonPropertyName("createdTimestamp")] public long? CreatedTimestamp { get; set; }

    [JsonPropertyName("credentials")] public CredentialRepresentationModel[]? Credentials { get; set; }

    [JsonPropertyName("disableableCredentialTypes")]
    public string[]? DisableableCredentialTypes { get; set; }

    [JsonPropertyName("email")] public string? Email { get; set; }

    [JsonPropertyName("emailVerified")] public bool? EmailVerified { get; set; }

    [JsonPropertyName("enabled")] public bool? Enabled { get; set; }

    [JsonPropertyName("federationLink")] public string? FederationLink { get; set; }

    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("groups")] public string[]? Groups { get; set; }

    [JsonPropertyName("firstName")] public string? FirstName { get; set; }

    [JsonPropertyName("lastName")] public string? LastName { get; set; }

    [JsonPropertyName("notBefore")] public int? NotBefore { get; set; }

    [JsonPropertyName("origin")] public string? Origin { get; set; }

    [JsonPropertyName("realmRoles")] public string[]? RealmRoles { get; set; }

    [JsonPropertyName("requiredActions")] public string[]? RequiredActions { get; set; }

    [JsonPropertyName("self")] public string? Self { get; set; }

    [JsonPropertyName("serviceAccountClientId")]
    public string? ServiceAccountClientId { get; set; }

    [JsonPropertyName("username")] public string? Username { get; set; }

    [JsonPropertyName("totp")] public bool? Totp { get; set; }

    internal static UserRepresentationModel FromUser(User user)
    {
        return new UserRepresentationModel
        {
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Email = user.Email.Value,
            Username = user.Email.Value,
            Enabled = true,
            EmailVerified = true,
            CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Attributes = new Dictionary<string, List<string>>(),
            RequiredActions = Array.Empty<string>(),
            DisableableCredentialTypes = Array.Empty<string>(),
            Groups = Array.Empty<string>(),
            RealmRoles = Array.Empty<string>()
        };
    }
}
 
