namespace Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
	public RefreshTokenCommandValidator()
	{
		RuleFor(x => x.RefreshToken)
		   .NotEmpty()
		   .WithMessage("The refresh token is required and cannot be empty.");
	}
}