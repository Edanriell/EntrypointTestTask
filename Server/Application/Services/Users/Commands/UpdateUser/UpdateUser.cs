using Domain.Entities;

namespace Application.Services.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand : IRequest<IResult>
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Username { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public byte[]? Photo { get; set; }
}

public class UpdateUserCommandHandler(UserManager<User> userManager) : IRequestHandler<UpdateUserCommand, IResult>
{
    public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found in database.");

        var user = await userManager.FindByIdAsync(request.Id);

        if (user is null)
            return TypedResults.NotFound($"User with ID {request.Id} was not found.");

        if (request.Name != null) user.Name = request.Name;
        if (request.Surname != null) user.Surname = request.Surname;
        if (request.Username != null) user.UserName = request.Username;
        if (request.Address != null) user.Address = request.Address;
        if (request.BirthDate.HasValue) user.BirthDate = request.BirthDate.Value;
        if (request.Photo != null) user.Photo = request.Photo;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return TypedResults.BadRequest($"User with ID {user.Id} could not be updated.");

        return TypedResults.Ok($"User with ID {user.Id} successfully updated.");
    }
}