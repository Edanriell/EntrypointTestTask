namespace Application.Users.Commands.ConfirmUserEmail;

public class ConfirmUserEmailCommandValidator : AbstractValidator<ConfirmUserEmailCommand>
{
	public ConfirmUserEmailCommandValidator()
	{
		RuleFor(x => x.UserId)
		   .NotEmpty()
		   .WithMessage("The User ID must not be empty.")
		   .MinimumLength(8)
		   .WithMessage("The User ID must be at least 8 characters in length.");

		RuleFor(x => x.Code)
		   .NotEmpty()
		   .WithMessage("Code must not be empty.")
		   .MinimumLength(16)
		   .WithMessage("Code must be at least 16 characters long.");

		RuleFor(x => x.ChangedEmail)
		   .EmailAddress()
		   .WithMessage("A valid email is required.");
	}
}