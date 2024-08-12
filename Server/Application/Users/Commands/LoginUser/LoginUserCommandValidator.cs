namespace Application.Users.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
	public LoginUserCommandValidator()
	{
		RuleFor(x => x.UseCookies)
		   .NotNull()
		   .WithMessage("Please specify whether cookies should be used.");

		RuleFor(x => x.UseSessionCookies)
		   .NotNull()
		   .WithMessage("Please specify whether session cookies should be used.");

		RuleFor(x => x.LoginCredentials)
		   .NotNull()
		   .WithMessage("Login credentials are required.");

		When(x => x.LoginCredentials != null, () =>
		{
			RuleFor(x => x.LoginCredentials!.UserName)
			   .NotEmpty()
			   .WithMessage("The username is required.")
			   .MinimumLength(4)
			   .WithMessage("The username must contain at least 4 characters.");

			RuleFor(x => x.LoginCredentials!.Password)
			   .NotEmpty()
			   .WithMessage("The password is required.")
			   .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
			   .WithMessage(
					"The password must be at least 8 characters in length, containing at least one uppercase letter, one lowercase letter, and one numeric digit.");

			RuleFor(x => x.LoginCredentials!.TwoFactorCode)
			   .MinimumLength(6)
			   .When(x => !string.IsNullOrEmpty(x.LoginCredentials?.TwoFactorCode))
			   .WithMessage("The two-factor authentication code must contain at least 6 characters.");

			RuleFor(x => x.LoginCredentials!.TwoFactorRecoveryCode)
			   .MinimumLength(6)
			   .When(x => !string.IsNullOrEmpty(x.LoginCredentials?.TwoFactorRecoveryCode))
			   .WithMessage("The two-factor recovery code must contain at least 6 characters.");
		});
	}
}