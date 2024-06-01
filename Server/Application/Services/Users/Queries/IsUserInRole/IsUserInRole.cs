using Domain.Entities;

namespace Application.Services.Users.Queries.IsUserInRole;

public record IsUserInRoleQuery : IRequest<IResult>
{
    public string UserId { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class IsUserInRoleQueryHandler(
    UserManager<User> userManager
)
    : IRequestHandler<IsUserInRoleQuery, IResult>
{
    public async Task<IResult> Handle(IsUserInRoleQuery request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null) return TypedResults.BadRequest();

        var result = await userManager.IsInRoleAsync(user, request.Role);

        return TypedResults.Ok(result);
    }
}