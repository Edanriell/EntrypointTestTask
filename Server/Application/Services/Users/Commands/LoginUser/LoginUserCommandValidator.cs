namespace Application.Services.Users.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.UseCookies)
            .NotNull()
            .WithMessage("Use cookies boolean value must be provided");

        RuleFor(x => x.UseSessionCookies)
            .NotNull()
            .WithMessage("Use session cookies boolean value must be provided");

        RuleFor(x => x.LoginCredentials!.UserName)
            .NotEmpty()
            .WithMessage("Username can't be empty")
            .MinimumLength(4)
            .WithMessage("Username must contain 4 or more letters");

        RuleFor(x => x.LoginCredentials!.Password)
            .NotEmpty()
            .WithMessage("Password can't be empty.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage(
                "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number");

        RuleFor(x => x.LoginCredentials!.TwoFactorCode)
            .MinimumLength(6)
            .WithMessage("Two factor code must contain at least 6 characters");

        RuleFor(x => x.LoginCredentials!.TwoFactorRecoveryCode)
            .MinimumLength(6)
            .WithMessage("Two factor recovery code must contain at least 6 characters");
    }
}