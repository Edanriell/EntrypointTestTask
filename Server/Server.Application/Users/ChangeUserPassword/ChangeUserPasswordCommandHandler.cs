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
        User? user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        if (string.IsNullOrEmpty(user.IdentityId))
        {
            return Result.Failure(UserErrors.IdentityNotFound);
        }

        try
        {
            await _authenticationService.ChangePasswordAsync(
                user.IdentityId,
                request.CurrentPassword,
                request.NewPassword,
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            // Handle specific authentication errors
            if (ex.Message.Contains("Current password is incorrect"))
            {
                return Result.Failure(UserErrors.InvalidCredentials);
            }

            return Result.Failure(UserErrors.PasswordChangeFailed);
        }
        catch (Exception)
        {
            return Result.Failure(UserErrors.PasswordChangeFailed);
        }
 
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
