using Domain.Entities;

namespace Application.Services.Users.Commands.DeleteUser;

public record DeleteUserCommand : IRequest<IResult>
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
}

public class DeleteUserCommandHandler(
    UserManager<User> userManager
)
    : IRequestHandler<DeleteUserCommand, IResult>
{
    public async Task<IResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null) TypedResults.BadRequest();

        if (user!.UserName != request.UserName)
            return TypedResults.BadRequest("Provided invalid username");

        var result = await userManager.DeleteAsync(user!);

        return TypedResults.Ok(result);
    }
}