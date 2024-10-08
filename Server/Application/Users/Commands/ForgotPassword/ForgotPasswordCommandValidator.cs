namespace Application.Users.Commands.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
	public ForgotPasswordCommandValidator()
	{
		RuleFor(x => x.Email)
		   .NotEmpty()
		   .WithMessage("Email address must not be empty.")
		   .EmailAddress()
		   .WithMessage("A valid email is required.");
	}
}