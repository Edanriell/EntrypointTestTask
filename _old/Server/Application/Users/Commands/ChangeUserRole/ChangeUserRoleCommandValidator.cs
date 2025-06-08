namespace Application.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
	public ChangeUserRoleCommandValidator()
	{
		RuleFor(x => x.UserId)
		   .NotEmpty()
		   .WithMessage("The User ID must not be empty.")
		   .MinimumLength(8)
		   .WithMessage("The User ID must be at least 8 characters in length.");

		RuleFor(x => x.Role)
		   .NotEmpty()
		   .WithMessage("The User role must not be empty.")
		   .Must(BeValidRole)
		   .WithMessage("Invalid user role. Role must be Customer, Moderator or Administrator.");
	}

	private bool BeValidRole(string role)
	{
		return role == "Customer" || role == "Moderator" || role == "Administrator";
	}
}