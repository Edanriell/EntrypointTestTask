using Domain.Entities;

namespace Application.Services.Users.Commands.LoginUser;

public record LoginUserCommand : IRequest<IResult>
{
    [FromQuery] public bool UseCookies { get; set; }
    [FromQuery] public bool UseSessionCookies { get; set; }
    [FromBody] public LoginCredentialsDto? LoginCredentials { get; init; }
}

public class LoginUserCommandHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager
)
    : IRequestHandler<LoginUserCommand, IResult>
{
    public async Task<IResult> Handle(LoginUserCommand request
        , CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var useCookieScheme = request.UseCookies || request.UseSessionCookies;
        var isPersistent = request.UseCookies && request.UseSessionCookies != true;
        signInManager.AuthenticationScheme =
            useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;
        // signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

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

        return Results.Empty;
    }
}

// EXAMPLE OF REQUEST
// {
// "userName": "NameSurname7",
// "password": "sqr##Z22g6#NAM"
// }