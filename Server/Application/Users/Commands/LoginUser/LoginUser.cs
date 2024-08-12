using Domain.Entities;

namespace Application.Users.Commands.LoginUser;

public sealed record LoginUserCommand : IRequest<IResult>
{
	[FromBody]  public LoginCredentialsDto? LoginCredentials  { get; set; }
	[FromQuery] public bool                 UseCookies        { get; set; }
	[FromQuery] public bool                 UseSessionCookies { get; set; }
}

public class LoginUserCommandHandler(
		UserManager<User>   userManager,
		SignInManager<User> signInManager
	)
	: IRequestHandler<LoginUserCommand, IResult>
{
	public async Task<IResult> Handle(LoginUserCommand  request
									, CancellationToken cancellationToken)
	{
		var useCookieScheme = request.UseCookies || request.UseSessionCookies;
		var isPersistent    = request.UseCookies && request.UseSessionCookies != true;
		signInManager.AuthenticationScheme =
			useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;
		// signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

		var result = await signInManager.PasswordSignInAsync(request.LoginCredentials!.UserName,
						 request.LoginCredentials.Password, isPersistent, true);

		if (result.RequiresTwoFactor)
		{
			if (!string.IsNullOrEmpty(request.LoginCredentials.TwoFactorCode))
				result = await signInManager.TwoFactorAuthenticatorSignInAsync(request.LoginCredentials.TwoFactorCode,
							 isPersistent,
							 isPersistent);
			else if (!string.IsNullOrEmpty(request.LoginCredentials.TwoFactorRecoveryCode))
				result = await signInManager.TwoFactorRecoveryCodeSignInAsync(request.LoginCredentials
							.TwoFactorRecoveryCode);
		}

		if (result.Succeeded)
		{
			var user = await userManager.FindByNameAsync(request.LoginCredentials.UserName);
			if (user is not null)
			{
				user.LastLogin = DateTime.Now;
				await userManager.UpdateAsync(user);
			}
		}

		return !result.Succeeded
				   ? TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized)
				   : Results.Empty;
	}
}