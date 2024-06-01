namespace Application.Services.Users.Queries.IsUserInRole;

public class IsUserInRoleQueryValidator : AbstractValidator<IsUserInRoleQuery>
{
    public IsUserInRoleQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id can't be empty")
            .MinimumLength(8)
            .WithMessage("User id must be at least 8 characters long");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("User role can't be empty")
            .MinimumLength(3)
            .WithMessage("User role name must contain at least 3 characters");
    }
}