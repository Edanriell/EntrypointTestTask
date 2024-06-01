namespace Application.Services.Users.Commands.UpdateUserInfo;

public class UpdateUserInfoCommandValidator : AbstractValidator<UpdateUserInfoCommand>
{
    public UpdateUserInfoCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .WithMessage("NewEmail can't be empty.")
            .EmailAddress()
            .WithMessage("Wrong email address format.");

        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage("Password can't be empty.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage(
                "OldPassword must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Password can't be empty.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage(
                "NewPassword must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");
    }
}