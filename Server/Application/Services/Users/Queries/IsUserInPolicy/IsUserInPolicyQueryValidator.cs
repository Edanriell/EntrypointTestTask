namespace Application.Services.Users.Queries.IsUserInPolicy;

public class IsUserInPolicyQueryValidator : AbstractValidator<IsUserInPolicyQuery>
{
    public IsUserInPolicyQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id can't be empty")
            .MinimumLength(8)
            .WithMessage("User id must be at least 8 characters long");

        RuleFor(x => x.PolicyName)
            .NotEmpty()
            .WithMessage("Policy name can't be empty")
            .MinimumLength(6)
            .WithMessage("Policy name must contain at least 6 characters");
    }
}