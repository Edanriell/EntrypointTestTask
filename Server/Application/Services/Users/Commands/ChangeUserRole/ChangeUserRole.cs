using Domain.Constants;
using Domain.Entities;

namespace Application.Services.Users.Commands.ChangeUserRole;

public record ChangeUserRoleCommand : IRequest<IResult>
{
    public string Role { get; set; } = null!;
    public string UserId { get; set; } = null!;
}

public class ChangeUserRole(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager) : IRequestHandler<ChangeUserRoleCommand, IResult>
{
    public async Task<IResult> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null)
            TypedResults.NotFound($"User with id {request.UserId} is not found.");

        if (request.Role == "Customer")
            if (!await roleManager.RoleExistsAsync(Roles.Customer))
                await roleManager.CreateAsync(new IdentityRole(Roles.Customer));

        if (request.Role == "Moderator")
            if (!await roleManager.RoleExistsAsync(Roles.Moderator))
                await roleManager.CreateAsync(new IdentityRole(Roles.Moderator));

        if (request.Role == "Administrator")
            if (!await roleManager.RoleExistsAsync(Roles.Administrator))
                await roleManager.CreateAsync(new IdentityRole(Roles.Administrator));

        var currentUserRoles = await userManager.GetRolesAsync(user!);
        await userManager.RemoveFromRolesAsync(user!, currentUserRoles);

        var result = await userManager.AddToRoleAsync(user!, request.Role);

        if (!result.Succeeded)
            return TypedResults.BadRequest(result.Errors);

        return TypedResults.Ok($"Role sucessfully changed to {request.Role}.");
    }
}