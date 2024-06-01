using Domain.Entities;

namespace Application.Services.Users.Queries.UserInfo;

public record UserInfoQuery : IRequest<IResult>
{
}

public class UserInfoQueryHandler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UserInfoQuery, IResult>
{
    public async Task<IResult> Handle(UserInfoQuery request,
        CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var claimsPrincipal = httpContextAccessor.HttpContext!.User;

        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();

        return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
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

// TODO
// THIS MUST BE TESTED