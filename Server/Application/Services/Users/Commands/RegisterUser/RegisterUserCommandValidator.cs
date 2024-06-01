namespace Application.Services.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name can't be empty")
            .MinimumLength(3)
            .WithMessage("Name must contain 3 letters or more");

        RuleFor(x => x.Surname)
            .NotEmpty()
            .WithMessage("Surname can't be empty")
            .MinimumLength(4)
            .WithMessage("Surname must contain 4 letters or more");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email can't be empty")
            .EmailAddress()
            .WithMessage("Wrong email address format");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username can't be empty")
            .MinimumLength(4)
            .WithMessage("Username must contain 4 or more letters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number can't be empty")
            .Matches(@"^\+\d{1,3}\d{9,}$")
            .WithMessage("Invalid phone number format. Please include country code");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password can't be empty")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage(
                "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address can't be empty")
            .MinimumLength(10)
            .WithMessage("Address must be at least 10 characters long");

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage("Date can't be empty")
            .Must(BeValidDate)
            .WithMessage("Wrong birth date format");

        RuleFor(x => x.Gender)
            .NotNull()
            .WithMessage("Gender can't be empty")
            .IsInEnum()
            .WithMessage("Gender type must be valid");

        RuleFor(x => x.Photo)
            .Matches(@"^(data:image\/(png|jpeg|jpg|gif);base64,)[A-Za-z0-9+/]+={0,2}$")
            .WithMessage("Invalid BASE64 image format");
    }

    private bool BeValidDate(DateTime date)
    {
        // Check if the date is within a reasonable range (e.g., not in the future)
        if (date > DateTime.Now) return false;

        // Check if it's a leap year
        if (date.Year % 4 != 0 || (date.Year % 100 == 0 && date.Year % 400 != 0))
        {
            if (date is { Month: 2, Day: > 28 }) return false; // Not a leap year, February has 28 days
        }
        else if (date is { Month: 2, Day: > 29 })
        {
            return false; // Leap year, but February has only 29 days
        }

        // Check month lengths
        if (date.Day < 1 ||
            date.Day > DateTime.DaysInMonth(date.Year, date.Month)) return false; // Invalid day for the given month

        // The date is valid 
        return true;
    }
}