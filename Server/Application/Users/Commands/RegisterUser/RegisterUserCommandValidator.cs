namespace Application.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
	public RegisterUserCommandValidator()
	{
		RuleFor(x => x.Name)
		   .NotEmpty()
		   .WithMessage("Name is required and cannot be empty.")
		   .Length(2, 50)
		   .WithMessage("Name must be between 2 and 50 characters.");

		RuleFor(x => x.Surname)
		   .NotEmpty()
		   .WithMessage("Surname is required and cannot be empty.")
		   .Length(2, 50)
		   .WithMessage("Surname must be between 2 and 50 characters.");

		RuleFor(x => x.Email)
		   .NotEmpty()
		   .WithMessage("Email is required and cannot be empty.")
		   .EmailAddress()
		   .WithMessage("A valid email is required.");

		RuleFor(x => x.UserName)
		   .NotEmpty()
		   .WithMessage("Username is required and cannot be empty.")
		   .Length(3, 50)
		   .WithMessage("Username must be between 3 and 50 characters.");

		RuleFor(x => x.PhoneNumber)
		   .NotEmpty()
		   .WithMessage("Phone Number is required and cannot be empty.")
		   .Matches(@"^\+\d{1,3}\d{9,}$")
		   .WithMessage("Please provide a valid phone number, including the country code.");

		RuleFor(x => x.Password)
		   .NotEmpty()
		   .WithMessage("Password is required and cannot be empty.")
		   .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
		   .WithMessage(
				"Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, and one number.");

		RuleFor(x => x.Address)
		   .NotEmpty()
		   .WithMessage("Address is required and cannot be empty.")
		   .MaximumLength(140)
		   .WithMessage("Address can't be longer than 100 characters.");

		RuleFor(x => x.BirthDate)
		   .NotEmpty()
		   .WithMessage("Birth Date is required and cannot be empty.")
		   .Must(BeValidDate)
		   .WithMessage("Please provide a valid birth date.");

		RuleFor(x => x.Gender)
		   .NotNull()
		   .WithMessage("Gender is required and cannot be empty.")
		   .IsInEnum()
		   .WithMessage("Please provide a valid gender.");

		RuleFor(x => x.Photo)
		   .Matches(@"^(data:image\/(png|jpeg|jpg|gif);base64,)[A-Za-z0-9+/]+={0,2}$")
		   .WithMessage("Please provide a valid BASE64 encoded image.");
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