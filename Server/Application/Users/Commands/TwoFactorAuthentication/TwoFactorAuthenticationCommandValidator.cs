namespace Application.Users.Commands.TwoFactorAuthentication;

public class TwoFactorAuthenticationCommandValidator : AbstractValidator<TwoFactorAuthenticationCommand>
{
	public TwoFactorAuthenticationCommandValidator()
	{
		RuleFor(x => x.Enable)
		   .NotNull()
		   .WithMessage("Enable is required and cannot be left empty.");

		RuleFor(x => x.ResetSharedKey)
		   .NotNull()
		   .WithMessage("Reset Shared Key is required and cannot be left empty.");

		RuleFor(x => x.ResetRecoveryCodes)
		   .NotNull()
		   .WithMessage("Reset Recovery Codes is required and cannot be left empty.");

		RuleFor(x => x.ForgetMachine)
		   .NotNull()
		   .WithMessage("Forget Machine is required and cannot be left empty.");

		When(x => x.Enable || x.ResetSharedKey || x.ResetRecoveryCodes, () =>
		{
			RuleFor(x => x.TwoFactorCode)
			   .NotEmpty()
			   .WithMessage("Two-Factor Code is required.")
			   .MinimumLength(6)
			   .WithMessage("Two-Factor Code must be at least 6 characters long.");
		});
	}
}