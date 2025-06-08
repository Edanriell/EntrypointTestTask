namespace Application.Users.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
	public DeleteUserCommandValidator()
	{
		RuleFor(x => x.UserId)
		   .NotEmpty()
		   .WithMessage("The User ID must not be empty.")
		   .MinimumLength(8)
		   .WithMessage("The User ID must be at least 8 characters in length.");

		RuleFor(x => x.UserName)
		   .NotEmpty()
		   .WithMessage("Username must not be empty.")
		   .Length(3, 50)
		   .WithMessage("Username must be between 3 and 50 characters.");
	}
}