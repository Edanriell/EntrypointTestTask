namespace Application.Services.Users.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id can't be empty")
            .MinimumLength(8)
            .WithMessage("User id must be at least 8 characters long");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username can't be empty")
            .MinimumLength(4)
            .WithMessage("Username must contain 4 or more letters");
    }
}