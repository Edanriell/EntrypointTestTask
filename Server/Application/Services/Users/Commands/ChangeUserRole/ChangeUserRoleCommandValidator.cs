namespace Application.Services.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id can't be empty")
            .MinimumLength(12)
            .WithMessage("User id length must be at least 12 characters long");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("User role can't be empty")
            .Must(BeValidRole)
            .WithMessage("Invalid user role. Role must be Customer, Administrator, or Moderator");
    }

    private bool BeValidRole(string role)
    {
        return role == "Customer" || role == "Moderator" || role == "Administrator";
    }
}