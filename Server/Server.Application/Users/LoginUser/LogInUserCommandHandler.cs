using Server.Application.Abstractions.Authentication;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Users;

namespace Server.Application.Users.LoginUser;
 
internal sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, AccessTokenResponse>
{
    private readonly IJwtService _jwtService;

    public LogInUserCommandHandler(IJwtService jwtService) { _jwtService = jwtService; }

    public async Task<Result<AccessTokenResponse>> Handle(
        LogInUserCommand request,
        CancellationToken cancellationToken)
    {
        Result<string> result = await _jwtService.GetAccessTokenAsync(
            request.Email,
            request.Password,
            cancellationToken
        );

        if (result.IsFailure)
        {
            return Result.Failure<AccessTokenResponse>(
                UserErrors.InvalidCredentials
            );
        }

        return new AccessTokenResponse(result.Value);
    }
}
