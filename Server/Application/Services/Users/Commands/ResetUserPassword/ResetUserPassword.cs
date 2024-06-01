using Application.Exceptions;
using Domain.Entities;

namespace Application.Services.Users.Commands.ResetUserPassword;

public record ResetUserPasswordCommand : IRequest<IResult>
{
    public string Email { get; set; } = null!;
    public string ResetCode { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ResetUserPasswordCommandHandler(
    UserManager<User> userManager)
    : IRequestHandler<ResetUserPasswordCommand, IResult>
{
    public async Task<IResult> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return ValidationProblemHelper.CreateValidationProblem(
                IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));

        IdentityResult result;
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
            result = await userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }

        if (!result.Succeeded) return ValidationProblemHelper.CreateValidationProblem(result);

        return TypedResults.Ok();
    }
}