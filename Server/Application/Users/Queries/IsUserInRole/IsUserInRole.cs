using Domain.Entities;

namespace Application.Users.Queries.IsUserInRole;

public record IsUserInRoleQuery : IRequest<IResult>
{
	public string UserId { get; set; } = string.Empty;
	public string Role   { get; set; } = string.Empty;
}

public class IsUserInRoleQueryHandler(
		UserManager<User> userManager
	)
	: IRequestHandler<IsUserInRoleQuery, IResult>
{
	public async Task<IResult> Handle(IsUserInRoleQuery request, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByIdAsync(request.UserId);

		if (user is null) return TypedResults.BadRequest();

		var result = await userManager.IsInRoleAsync(user, request.Role);

		return TypedResults.Ok(result);
	}
}