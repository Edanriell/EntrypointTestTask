using Application.Interfaces;

namespace Application.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    IUser user,
    IIdentityService identityService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = user.Id ?? string.Empty;
        var userName = string.Empty;

        if (!string.IsNullOrEmpty(userId)) userName = await identityService.GetUserNameAsync(userId);

        logger.LogInformation("API Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        // Call the next delegate/middleware in the pipeline
        return await next();
    }
}