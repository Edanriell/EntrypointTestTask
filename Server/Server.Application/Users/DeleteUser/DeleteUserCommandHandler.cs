using Server.Application.Abstractions.Authentication;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Users;

namespace Server.Application.Users.DeleteUser;

internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly UserService _userService;

    public DeleteUserCommandHandler(
        IAuthenticationService authenticationService,
        UserService userService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _authenticationService = authenticationService;
        _userService = userService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        Result deleteResult = await _userService.DeleteUserAsync(user, cancellationToken);
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        // Delete user from Keycloak
        if (!string.IsNullOrEmpty(user.IdentityId))
        {
            try
            {
                await _authenticationService.DeleteUserAsync(
                    user.IdentityId,
                    cancellationToken);
            }
            catch (Exception)
            {
                return Result.Failure(UserErrors.DeleteFailed);
            }
        }

        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
