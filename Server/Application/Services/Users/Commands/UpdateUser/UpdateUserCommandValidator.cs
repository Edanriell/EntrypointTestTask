namespace Application.Services.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Name)
            .MaximumLength(50)
            .WithMessage("Name cannot be longer than 50 characters.");

        RuleFor(x => x.Surname)
            .MaximumLength(50)
            .WithMessage("Surname cannot be longer than 50 characters.");

        RuleFor(x => x.Username)
            .MinimumLength(4)
            .WithMessage("Username must be at least 4 characters long.")
            .MaximumLength(20)
            .WithMessage("Username cannot be longer than 20 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(100)
            .WithMessage("Address cannot be longer than 100 characters.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now)
            .WithMessage("Birth date cannot be in the future.")
            .GreaterThan(DateTime.Now.AddYears(-120))
            .WithMessage("Birth date is not valid.");

        RuleFor(x => x.Photo)
            .Must(photo => photo == null || photo.Length <= 5 * 1024 * 1024) // 5 MB limit
            .WithMessage("Photo size cannot exceed 5 MB.");
    }
}