using FluentValidation;

namespace Server.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in valid international format");

        RuleFor(c => c.Gender)
            .NotEmpty()
            .IsInEnum();

        RuleFor(c => c.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.ZipCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(c => c.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Must(ContainUppercase)
            .WithMessage("Password must contain at least one uppercase letter")
            .Must(ContainLowercase)
            .WithMessage("Password must contain at least one lowercase letter")
            .Must(ContainDigit)
            .WithMessage("Password must contain at least one digit")
            .Must(ContainSpecialCharacter)
            .WithMessage("Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?)")
            .MaximumLength(128)
            .WithMessage("Password cannot exceed 128 characters");
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
