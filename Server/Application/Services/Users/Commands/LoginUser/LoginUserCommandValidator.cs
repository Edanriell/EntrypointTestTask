namespace Application.Services.Users.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.UseCookies)
            .NotNull()
            .WithMessage("The 'UseCookies' field is required.");

        RuleFor(x => x.UseSessionCookies)
            .NotNull()
            .WithMessage("The 'UseSessionCookies' field is required.");

        RuleFor(x => x.LoginCredentials)
            .NotNull()
            .WithMessage("Login credentials must be provided.");

        When(x => x.LoginCredentials != null, () =>
        {
            RuleFor(x => x.LoginCredentials!.UserName)
                .NotEmpty()
                .WithMessage("The username is required.")
                .MinimumLength(4)
                .WithMessage("The username must be at least 4 characters long.");

            RuleFor(x => x.LoginCredentials!.Password)
                .NotEmpty()
                .WithMessage("The password is required.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
                .WithMessage(
                    "The password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");

            RuleFor(x => x.LoginCredentials!.TwoFactorCode)
                .MinimumLength(6)
                .When(x => !string.IsNullOrEmpty(x.LoginCredentials?.TwoFactorCode))
                .WithMessage("The two-factor code must be at least 6 characters long.");

            RuleFor(x => x.LoginCredentials!.TwoFactorRecoveryCode)
                .MinimumLength(6)
                .When(x => !string.IsNullOrEmpty(x.LoginCredentials?.TwoFactorRecoveryCode))
                .WithMessage("The two-factor recovery code must be at least 6 characters long.");
        });
    }
}