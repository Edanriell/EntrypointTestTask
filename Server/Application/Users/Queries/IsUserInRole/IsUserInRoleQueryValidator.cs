namespace Application.Users.Queries.IsUserInRole;

public class IsUserInRoleQueryValidator : AbstractValidator<IsUserInRoleQuery>
{
	public IsUserInRoleQueryValidator()
	{
		RuleFor(x => x.UserId)
		   .NotEmpty()
		   .WithMessage("The User ID must not be empty.")
		   .MinimumLength(8)
		   .WithMessage("The User ID must be at least 8 characters in length.");

		RuleFor(x => x.Role)
		   .NotEmpty()
		   .WithMessage("The User role must not be empty.")
		   .MinimumLength(3)
		   .WithMessage("The User role name must contain at least 3 characters.");
	}
}