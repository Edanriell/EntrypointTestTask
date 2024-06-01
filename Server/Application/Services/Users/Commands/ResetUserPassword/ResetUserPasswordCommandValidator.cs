namespace Application.Services.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email can't be empty")
            .EmailAddress()
            .WithMessage("Provided invalid email");

        RuleFor(x => x.ResetCode)
            .NotEmpty()
            .WithMessage("ResetCode can't be empty")
            .MinimumLength(16)
            .WithMessage("ResetCode is too short");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Password can't be empty")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage(
                "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number");
    }
}