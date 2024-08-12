namespace Application.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
	public ResetUserPasswordCommandValidator()
	{
		RuleFor(x => x.Email)
		   .NotEmpty()
		   .WithMessage("Email is required and cannot be empty.")
		   .EmailAddress()
		   .WithMessage("A valid email is required.");

		RuleFor(x => x.ResetCode)
		   .NotEmpty()
		   .WithMessage("Reset Code is required and cannot be left empty.")
		   .MinimumLength(16)
		   .WithMessage("Reset Code is too short. It must be at least 16 characters long.");

		RuleFor(x => x.NewPassword)
		   .NotEmpty()
		   .WithMessage("New Password is required and cannot be left empty.")
		   .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
		   .WithMessage(
				"New Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, and one number.");
	}
}