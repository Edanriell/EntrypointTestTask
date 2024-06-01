namespace Application.Services.Users.Commands.ResendConfirmationEmail;

public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmationEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email can't be empty")
            .EmailAddress()
            .WithMessage("Provided invalid email");
    }
}