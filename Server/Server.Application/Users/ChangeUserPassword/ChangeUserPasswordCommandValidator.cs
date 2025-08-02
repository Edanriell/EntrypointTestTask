using FluentValidation;

namespace Server.Application.Users.ChangeUserPassword;

internal sealed class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.")
            .MinimumLength(1)
            .WithMessage("Current password cannot be empty.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters long.")
            .Must(ContainUppercase)
            .WithMessage("New Password must contain at least one uppercase letter")
            .Must(ContainLowercase)
            .WithMessage("New Password must contain at least one lowercase letter")
            .Must(ContainDigit)
            .WithMessage("New Password must contain at least one digit")
            .Must(ContainSpecialCharacter)
            .WithMessage("New Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?)")
            .MaximumLength(128)
            .WithMessage("New Password cannot exceed 128 characters");

        RuleFor(x => x)
            .Must(x => x.CurrentPassword != x.NewPassword)
            .WithMessage("New password must be different from current password.");
    }

    private static bool ContainUppercase(string password)
    {
        return password.Any(char.IsUpper);
    }

    private static bool ContainLowercase(string password)
    {
        return password.Any(char.IsLower);
    }

    private static bool ContainDigit(string password)
    {
        return password.Any(char.IsDigit);
    }

    private static bool ContainSpecialCharacter(string password)
    {
        const string specialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        return password.Any(specialCharacters.Contains);
    }
}
 
 
