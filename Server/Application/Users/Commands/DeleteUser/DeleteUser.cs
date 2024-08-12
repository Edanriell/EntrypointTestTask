using Domain.Entities;

namespace Application.Users.Commands.DeleteUser;

public record DeleteUserCommand : IRequest<IResult>
{
	public string UserId   { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
}

public class DeleteUserCommandHandler(
		UserManager<User> userManager
	)
	: IRequestHandler<DeleteUserCommand, IResult>
{
	public async Task<IResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByIdAsync(request.UserId);

		if (user is null) TypedResults.BadRequest();

		if (user!.UserName != request.UserName)
			return TypedResults.BadRequest("Provided invalid username.");

		var result = await userManager.DeleteAsync(user);

		return TypedResults.Ok(result);
	}
}