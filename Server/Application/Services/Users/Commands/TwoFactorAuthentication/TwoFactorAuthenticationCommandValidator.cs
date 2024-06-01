namespace Application.Services.Users.Commands.TwoFactorAuthentication;

public class TwoFactorAuthenticationCommandValidator : AbstractValidator<TwoFactorAuthenticationCommand>
{
    public TwoFactorAuthenticationCommandValidator()
    {
        RuleFor(x => x.Enable)
            .NotNull()
            .WithMessage("Enable must be provided.");

        RuleFor(x => x.ResetSharedKey)
            .NotNull()
            .WithMessage("ResetSharedKey must be provided.");

        RuleFor(x => x.ResetRecoveryCodes)
            .NotNull()
            .WithMessage("ResetRecoveryCodes must be provided.");

        RuleFor(x => x.ForgetMachine)
            .NotNull()
            .WithMessage("ForgetMachine must be provided.");

        RuleFor(x => x.TwoFactorCode)
            .MinimumLength(16)
            .WithMessage("TwoFactorCode must be at least 16 characters long.");
    }
}