using Domain.Entities;

namespace Application.Users.Commands.ResendConfirmationEmail;

public record ResendConfirmationEmailCommand : IRequest<IResult>
{
	public string Email { get; set; } = string.Empty;
}

public class ResendConfirmationEmail(
	UserManager<User>    userManager,
	IEmailSender<User>   emailSender,
	LinkGenerator        linkGenerator,
	IHttpContextAccessor httpContextAccessor) : IRequestHandler<ResendConfirmationEmailCommand, IResult>
{
	public async Task<IResult> Handle(ResendConfirmationEmailCommand request,
									  CancellationToken              cancellationToken)
	{
		if (await userManager.FindByEmailAsync(request.Email) is not { } user) return TypedResults.Ok();

		await SendConfirmationEmailAsync(user, httpContextAccessor.HttpContext!, request.Email);
		return TypedResults.Ok();
	}

	private async Task SendConfirmationEmailAsync(User user, HttpContext context, string email, bool isChange = false)
	{
		var code = isChange
					   ? await userManager.GenerateChangeEmailTokenAsync(user, email)
					   : await userManager.GenerateEmailConfirmationTokenAsync(user);

		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		var userId        = await userManager.GetUserIdAsync(user);
		var userIdEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));

		var routeValues = new RouteValueDictionary
						  {
							  ["userId"] = userIdEncoded,
							  ["code"]   = code
						  };

		// Console.WriteLine(userIdEncoded);
		// Console.WriteLine(code);

		var confirmEmailUrl =
			linkGenerator.GetUriByName(context, "ConfirmUserEmail", routeValues)
		 ?? throw new NotSupportedException(
				"Could not find endpoint named 'ConfirmUserEmail'.");

		// Console.WriteLine(confirmEmailUrl);

		// Static endpoint is bad 
		// Also emailSender is not configured yet 
		await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
	}
}