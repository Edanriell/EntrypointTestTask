namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
	public UpdateUserCommandValidator()
	{
		RuleFor(x => x.Id)
		   .NotEmpty()
		   .WithMessage("User ID is required and cannot be empty.");

		RuleFor(x => x.Name)
		   .MaximumLength(50)
		   .WithMessage("Name must not exceed 50 characters in length.");

		RuleFor(x => x.Surname)
		   .MaximumLength(50)
		   .WithMessage("Surname must not exceed 50 characters in length.");

		RuleFor(x => x.Username)
		   .MinimumLength(4)
		   .WithMessage("Username must be at least 4 characters in length.")
		   .MaximumLength(20)
		   .WithMessage("Username must not exceed 20 characters in length.");

		RuleFor(x => x.Address)
		   .MaximumLength(140)
		   .WithMessage("Address must not exceed 140 characters in length.");

		RuleFor(x => x.BirthDate)
		   .LessThan(DateTime.Now)
		   .WithMessage("Birth Date cannot be a future date.")
		   .GreaterThan(DateTime.Now.AddYears(-120))
		   .WithMessage("Birth Date is not valid. It must be within the last 120 years.");

		RuleFor(x => x.Photo)
		   .Must(photo => photo == null || IsValidPhotoSize(photo))
		   .WithMessage("Photo size must not exceed 5 MB.");
	}

	private bool IsValidPhotoSize(string photo)
	{
		const int maxPhotoSizeBytes = 5 * 1024 * 1024; // 5 MB
		return photo.Length <= maxPhotoSizeBytes;
	}
}