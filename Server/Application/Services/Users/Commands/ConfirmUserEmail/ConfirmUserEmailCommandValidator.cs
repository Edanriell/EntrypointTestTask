namespace Application.Services.Users.Commands.ConfirmUserEmail;

public class ConfirmUserEmailCommandValidator : AbstractValidator<ConfirmUserEmailCommand>
{
    public ConfirmUserEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id can't be empty")
            .MinimumLength(16)
            .WithMessage("User id is too short");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code can't be empty")
            .MinimumLength(16)
            .WithMessage("Code is too short");

        RuleFor(x => x.ChangedEmail)
            .EmailAddress()
            .WithMessage("Provided invalid email");
    }
}