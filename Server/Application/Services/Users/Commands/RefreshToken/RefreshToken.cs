using Domain.Entities;

namespace Application.Services.Users.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<IResult>
{
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
    TimeProvider timeProvider) : IRequestHandler<RefreshTokenCommand, IResult>
{
    public async Task<IResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users is null) return TypedResults.NotFound("No users has been found");

        var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(request.RefreshToken);

        if (refreshTicket is null) return Results.BadRequest("Invalid refresh token provided.");

        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)
            return TypedResults.Challenge();

        var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);

        return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }
}