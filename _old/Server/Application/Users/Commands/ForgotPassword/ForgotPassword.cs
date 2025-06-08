using Domain.Entities;

namespace Application.Users.Commands.ForgotPassword;

public record ForgotPasswordCommand : IRequest<IResult>
{
	public string Email { get; set; } = null!;
}

public class ForgotPasswordCommandHandler(
	UserManager<User>  userManager,
	IEmailSender<User> emailSender)
	: IRequestHandler<ForgotPasswordCommand, IResult>
{
	public async Task<IResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByEmailAsync(request.Email);

		if (user is not null && await userManager.IsEmailConfirmedAsync(user))
		{
			var code = await userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			await emailSender.SendPasswordResetCodeAsync(user, request.Email, HtmlEncoder.Default.Encode(code));
		}

		return TypedResults.Ok();
	}
}