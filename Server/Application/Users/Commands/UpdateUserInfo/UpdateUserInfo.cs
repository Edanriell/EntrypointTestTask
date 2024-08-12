using Application.Exceptions;
using Domain.Entities;

namespace Application.Users.Commands.UpdateUserInfo;

public record UpdateUserInfoCommand : IRequest<IResult>
{
	public string NewEmail    { get; set; } = string.Empty;
	public string OldPassword { get; set; } = string.Empty;
	public string NewPassword { get; set; } = string.Empty;
}

public class UpdateUserInfoCommandHandler(
		UserManager<User>    userManager,
		IEmailSender<User>   emailSender,
		LinkGenerator        linkGenerator,
		IHttpContextAccessor httpContextAccessor
	)
	: IRequestHandler<UpdateUserInfoCommand, IResult>
{
	public async Task<IResult> Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
	{
		var claimsPrincipal = httpContextAccessor.HttpContext!.User;

		if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();

		if (!string.IsNullOrEmpty(request.NewEmail))
			return ValidationProblemHelper.CreateValidationProblem(
				IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(request.NewEmail)));

		if (!string.IsNullOrEmpty(request.NewPassword))
		{
			if (string.IsNullOrEmpty(request.OldPassword))
				return ValidationProblemHelper.CreateValidationProblem("OldPasswordRequired",
					"The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");

			var changePasswordResult =
				await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
			if (!changePasswordResult.Succeeded)
				return ValidationProblemHelper.CreateValidationProblem(changePasswordResult);
		}

		if (!string.IsNullOrEmpty(request.NewEmail))
		{
			var email = await userManager.GetEmailAsync(user);

			if (email != request.NewEmail)
				await SendConfirmationEmailAsync(user, httpContextAccessor.HttpContext!, request.NewEmail, true);
		}

		return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
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
		// Also i'm not sure if emailSender will work, it is not configured yet 
		await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
	}

	private static async Task<InfoResponse> CreateInfoResponseAsync<TUser>(TUser user, UserManager<TUser> userManager)
		where TUser : class
	{
		return new InfoResponse
			   {
				   Email = await userManager.GetEmailAsync(user) ??
						   throw new NotSupportedException("Users must have an email."),
				   IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user)
			   };
	}
}