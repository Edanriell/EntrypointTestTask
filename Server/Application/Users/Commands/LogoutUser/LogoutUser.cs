using Domain.Entities;

namespace Application.Users.Commands.LogoutUser;

public record LogoutUserCommand : IRequest<IResult>
{
}

public class LogoutUserCommandHandler(
	UserManager<User>   userManager,
	SignInManager<User> userSignInManager)
	: IRequestHandler<LogoutUserCommand, IResult>
{
	public async Task<IResult> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
	{
		await userSignInManager.SignOutAsync();
		return Results.Empty;
	}
}