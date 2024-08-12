using Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Application.Users.Queries.IsUserInPolicy;

public record IsUserInPolicyQuery : IRequest<IResult>
{
	public string UserId     { get; set; } = string.Empty;
	public string PolicyName { get; set; } = string.Empty;
}

public class IsUserInPolicyQueryHandler(
		UserManager<User>                 userManager,
		IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory,
		IAuthorizationService             authorizationService
	)
{
	public async Task<IResult> Handle(IsUserInPolicyQuery request, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByIdAsync(request.UserId);

		if (user == null) return TypedResults.BadRequest();

		var principal = await userClaimsPrincipalFactory.CreateAsync(user);

		var result = await authorizationService.AuthorizeAsync(principal, request.PolicyName);

		return TypedResults.Ok(result);
	}
}