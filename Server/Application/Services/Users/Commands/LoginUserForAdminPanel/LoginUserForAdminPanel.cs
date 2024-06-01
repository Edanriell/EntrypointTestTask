using Domain.Constants;
using Domain.Entities;

namespace Application.Services.Users.Commands.LoginUserForAdminPanel;

public record LoginUserForAdminPanelCommand : IRequest<IResult>
{
    [FromQuery] public bool UseCookies { get; set; }
    [FromQuery] public bool UseSessionCookies { get; set; }
    [FromBody] public LoginUserForAdminPanelCredentialsDto? LoginCredentials { get; init; }
}

public class LoginUserForAdminPanelCommandHandler(
    SignInManager<User> signInManager,
    UserManager<User> userManager
)
    : IRequestHandler<LoginUserForAdminPanelCommand, IResult>
{
    public async Task<IResult> Handle(LoginUserForAdminPanelCommand request
        , CancellationToken cancellationToken)
    {
        var useCookieScheme = request.UseCookies || request.UseSessionCookies;
        var isPersistent = request.UseCookies && request.UseSessionCookies != true;
        signInManager.AuthenticationScheme =
            useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        var user = await userManager.FindByNameAsync(request.LoginCredentials!.UserName);
        if (user == null)
            return TypedResults.Problem("Invalid credentials", statusCode: StatusCodes.Status401Unauthorized);

        var result = await signInManager.PasswordSignInAsync(request.LoginCredentials!.UserName,
            request.LoginCredentials.Password, isPersistent, true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(request.LoginCredentials.TwoFactorCode))
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(request.LoginCredentials.TwoFactorCode,
                    isPersistent,
                    isPersistent);
            else if (!string.IsNullOrEmpty(request.LoginCredentials.TwoFactorRecoveryCode))
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(request.LoginCredentials
                    .TwoFactorRecoveryCode);
        }

        if (!result.Succeeded)
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);

        // Check if the user has the admin role
        var isModerator = await userManager.IsInRoleAsync(user, Roles.Moderator);
        var isAdministrator = await userManager.IsInRoleAsync(user, Roles.Administrator);
        var isSuperAdministrator = await userManager.IsInRoleAsync(user, Roles.SuperAdministrator);

        if (!isModerator && !isAdministrator && !isSuperAdministrator)
        {
            await signInManager.SignOutAsync();
            return TypedResults.Problem("Access denied", statusCode: StatusCodes.Status403Forbidden);
        }

        return Results.Empty;
    }
}

// EXAMPLE OF REQUEST
// {
// "userName": "NameSurname7",
// "password": "sqr##Z22g6#NAM"
// }