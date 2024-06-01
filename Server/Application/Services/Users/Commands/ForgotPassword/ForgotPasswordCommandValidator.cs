namespace Application.Services.Users.Commands.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email can't be empty.")
            .EmailAddress()
            .WithMessage("Provided invalid email.");
    }
}