using Domain.Entities;

namespace Application.Services.Users.Commands.ConfirmUserEmail;

public record ConfirmUserEmailCommand : IRequest<IResult>
{
    public string UserId { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? ChangedEmail { get; set; }
}

public class ConfirmUserEmailCommandHandler(
    UserManager<User> userManager)
    : IRequestHandler<ConfirmUserEmailCommand, IResult>
{
    public async Task<IResult> Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var userIdDecoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.UserId));

        var user = await userManager.FindByIdAsync(userIdDecoded);

        if (user is null) return TypedResults.Unauthorized();

        if (await userManager.IsEmailConfirmedAsync(user)) return Results.BadRequest("Email is already confirmed.");

        try
        {
            request.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        }
        catch (FormatException)
        {
            return TypedResults.Unauthorized();
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(request.ChangedEmail))
            result = await userManager.ConfirmEmailAsync(user, request.Code);
        else
            result = await userManager.ChangeEmailAsync(user, request.ChangedEmail, request.Code);
        // If UserName is the same as Email we need to change Name also.
        // if (result.Succeeded) result = await userManager.SetUserNameAsync(user, request.ChangedEmail);

        if (!result.Succeeded) return TypedResults.Unauthorized();

        return TypedResults.Text("Thank you for confirming your email.");
    }
}