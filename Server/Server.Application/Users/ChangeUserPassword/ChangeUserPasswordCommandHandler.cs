using Server.Application.Abstractions.Authentication;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Users;

namespace Server.Application.Users.ChangeUserPassword;

internal sealed class ChangeUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public ChangeUserPasswordCommandHandler(
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ChangeUserPasswordCommand request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[DEBUG] Starting password change for user ID: {request.UserId}");

        User? user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            Console.WriteLine($"[ERROR] User not found with ID: {request.UserId}");
            return Result.Failure(UserErrors.NotFound);
        }

        Console.WriteLine($"[DEBUG] User found: {user.Email}, Identity ID: {user.IdentityId}");

        if (string.IsNullOrEmpty(user.IdentityId))
        {
            Console.WriteLine($"[ERROR] User identity ID is null or empty for user: {user.Email}");
            return Result.Failure(UserErrors.IdentityNotFound);
        }

        try
        {
            Console.WriteLine(
                $"[DEBUG] Calling authentication service to change password for identity: {user.IdentityId}");

            await _authenticationService.ChangePasswordAsync(
                user.IdentityId,
                request.CurrentPassword,
                request.NewPassword,
                cancellationToken);

            Console.WriteLine($"[DEBUG] Password change successful for user: {user.Email}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[ERROR] InvalidOperationException during password change: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");

            // Handle specific authentication errors
            if (ex.Message.Contains("Current password is incorrect"))
            {
                return Result.Failure(UserErrors.InvalidCredentials);
            }

            return Result.Failure(UserErrors.PasswordChangeFailed);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[ERROR] Unexpected exception during password change: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            return Result.Failure(UserErrors.PasswordChangeFailed);
        }

        try
        {
            // Console.WriteLine($"[DEBUG] Saving changes to database");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // Console.WriteLine($"[DEBUG] Changes saved successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to save changes: {ex.Message}");
            return Result.Failure(UserErrors.PasswordChangeFailed);
        }

        return Result.Success();
    }
}
