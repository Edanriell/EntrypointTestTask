using Domain.Entities;

namespace Application.Services.Users.Commands.LogoutUser;

public record LogoutUserCommand : IRequest<IResult>
{
}

public class LogoutUserCommandHandler(
    UserManager<User> userManager,
    SignInManager<User> userSignInManager)
    : IRequestHandler<LogoutUserCommand, IResult>
{
    public async Task<IResult> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        await userSignInManager.SignOutAsync();
        return Results.Empty;
    }
}