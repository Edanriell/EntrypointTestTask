namespace Application.Users.Commands.UpdateUserInfo;

public class UpdateUserInfoCommandValidator : AbstractValidator<UpdateUserInfoCommand>
{
	public UpdateUserInfoCommandValidator()
	{
		RuleFor(x => x.NewEmail)
		   .NotEmpty()
		   .WithMessage("The new email address is required and cannot be empty.")
		   .EmailAddress()
		   .WithMessage("The provided email address is not in a valid format.");

		RuleFor(x => x.OldPassword)
		   .NotEmpty()
		   .WithMessage("The current password is required and cannot be empty.")
		   .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
		   .WithMessage(
				"The current password must be at least 8 characters in length and contain at least one uppercase letter, one lowercase letter, and one digit.");

		RuleFor(x => x.NewPassword)
		   .NotEmpty()
		   .WithMessage("The new password is required and cannot be empty.")
		   .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
		   .WithMessage(
				"The new password must be at least 8 characters in length and contain at least one uppercase letter, one lowercase letter, and one digit.");
	}
}