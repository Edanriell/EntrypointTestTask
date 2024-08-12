namespace Application.Users.Queries.IsUserInPolicy;

public class IsUserInPolicyQueryValidator : AbstractValidator<IsUserInPolicyQuery>
{
	public IsUserInPolicyQueryValidator()
	{
		RuleFor(x => x.UserId)
		   .NotEmpty()
		   .WithMessage("The User ID must not be empty.")
		   .MinimumLength(8)
		   .WithMessage("The User ID must be at least 8 characters in length.");

		RuleFor(x => x.PolicyName)
		   .NotEmpty()
		   .WithMessage("The Policy Name must not be empty.")
		   .MinimumLength(6)
		   .WithMessage("The Policy Name must contain at least 6 characters.");
	}
}