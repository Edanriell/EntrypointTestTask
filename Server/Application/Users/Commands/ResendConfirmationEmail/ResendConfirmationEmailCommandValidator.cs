namespace Application.Users.Commands.ResendConfirmationEmail;

public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
	public ResendConfirmationEmailCommandValidator()
	{
		RuleFor(x => x.Email)
		   .NotEmpty()
		   .WithMessage("Email is required and cannot be empty.")
		   .EmailAddress()
		   .WithMessage("A valid email is required.");
	}
}