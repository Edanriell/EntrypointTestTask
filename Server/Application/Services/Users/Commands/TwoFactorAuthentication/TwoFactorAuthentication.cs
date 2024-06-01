using Application.Exceptions;
using Domain.Entities;

namespace Application.Services.Users.Commands.TwoFactorAuthentication;

public record TwoFactorAuthenticationCommand : IRequest<IResult>
{
    public bool Enable { get; set; } // Indicates whether to enable or disable two-factor authentication
    public bool ResetSharedKey { get; set; } // Indicates whether to reset the shared key for two-factor authentication
    public string? TwoFactorCode { get; set; } // Two-factor authentication code provided by the user
    public bool ResetRecoveryCodes { get; set; } // Indicates whether to reset recovery codes
    public bool ForgetMachine { get; set; } // Indicates whether to forget the machine
}

public class TwoFactorAuthenticationCommandHandler(
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor,
    SignInManager<User> signInManager
)
    : IRequestHandler<TwoFactorAuthenticationCommand, IResult>
{
    public async Task<IResult> Handle(TwoFactorAuthenticationCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var claimsPrincipal = httpContextAccessor.HttpContext!.User;

        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();

        if (request.Enable)
        {
            if (request.ResetSharedKey)
                return ValidationProblemHelper.CreateValidationProblem("CannotResetSharedKeyAndEnable",
                    "Resetting the 2fa shared key must disable 2fa until a 2fa token based on the new shared key is validated.");
            if (string.IsNullOrEmpty(request.TwoFactorCode))
                return ValidationProblemHelper.CreateValidationProblem("RequiresTwoFactor",
                    "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.");
            if (!await userManager.VerifyTwoFactorTokenAsync(user,
                    userManager.Options.Tokens.AuthenticatorTokenProvider, request.TwoFactorCode))
                return ValidationProblemHelper.CreateValidationProblem("InvalidTwoFactorCode",
                    "The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa.");

            await userManager.SetTwoFactorEnabledAsync(user, true);
        }
        else if (request.Enable == false || request.ResetSharedKey)
        {
            await userManager.SetTwoFactorEnabledAsync(user, false);
        }

        if (request.ResetSharedKey) await userManager.ResetAuthenticatorKeyAsync(user);

        string[]? recoveryCodes = null;
        if (request.ResetRecoveryCodes || (request.Enable && await userManager.CountRecoveryCodesAsync(user) == 0))
        {
            var recoveryCodesEnumerable = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            recoveryCodes = recoveryCodesEnumerable?.ToArray();
        }

        if (request.ForgetMachine) await signInManager.ForgetTwoFactorClientAsync();

        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            key = await userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(key))
                throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
        }

        return TypedResults.Ok(new TwoFactorResponse
        {
            SharedKey = key,
            RecoveryCodes = recoveryCodes,
            RecoveryCodesLeft = recoveryCodes?.Length ?? await userManager.CountRecoveryCodesAsync(user),
            IsTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
            IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user)
        });
    }
}