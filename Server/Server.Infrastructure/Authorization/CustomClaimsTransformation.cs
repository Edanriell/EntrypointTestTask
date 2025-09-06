using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Server.Domain.Users;
using Server.Infrastructure.Authentication;

namespace Server.Infrastructure.Authorization;

internal sealed class CustomClaimsTransformation : IClaimsTransformation
{
    private readonly IServiceProvider _serviceProvider;

    public CustomClaimsTransformation(IServiceProvider serviceProvider) { _serviceProvider = serviceProvider; }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        bool notAuthenticated = principal.Identity is not { IsAuthenticated: true };
        bool hasRequiredClaims =
            principal.HasClaim(c => c.Type == ClaimTypes.Role) &&
            principal.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub);

        if (notAuthenticated || hasRequiredClaims)
        {
            return principal;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();

        AuthorizationService authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        string identityId = principal.GetIdentityId();

        UserRolesResponse userRoles = await authorizationService.GetRolesForUserAsync(
            identityId
        );

        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userRoles.UserId.ToString()
            )
        );

        foreach (Role role in userRoles.Roles)
        {
            claimsIdentity.AddClaim(
                new Claim(
                    ClaimTypes.Role,
                    role.Name
                )
            );
        }

        principal.AddIdentity(
            claimsIdentity
        );

        return principal;
    }
}
 
