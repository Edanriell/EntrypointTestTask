using Domain.Entities;

namespace Application.Users.Queries.UserInfo;

public record UserInfoQuery : IRequest<IResult>
{
}

public class UserInfoQueryHandler(
	UserManager<User>    userManager,
	IHttpContextAccessor httpContextAccessor,
	IMapper              mapper)
	: IRequestHandler<UserInfoQuery, IResult>
{
	public async Task<IResult> Handle(UserInfoQuery     request,
									  CancellationToken cancellationToken)
	{
		var claimsPrincipal = httpContextAccessor.HttpContext!.User;

		if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();

		var userInfoDto = await CreateInfoResponseAsync(user, userManager, mapper);

		return TypedResults.Ok(userInfoDto);
	}

	private static async Task<UserInfoQueryResponseDto> CreateInfoResponseAsync(User user,
		UserManager<User>                                                            userManager,
		IMapper                                                                      mapper)
	{
		var userInfoDto = mapper.Map<UserInfoQueryResponseDto>(user);

		userInfoDto.Email = await userManager.GetEmailAsync(user)
						 ?? throw new NotSupportedException("Users must have an email.");

		userInfoDto.IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);

		userInfoDto.Roles = await userManager.GetRolesAsync(user);

		return userInfoDto;
	}
}