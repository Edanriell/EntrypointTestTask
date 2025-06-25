using Server.Application.Abstractions.Authentication;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Shared;
using Server.Domain.Users;

namespace Server.Application.Users.UpdateUser;

// Doubt that user update command works, probably there will be issues with keycloak

internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        // Create value objects only if values are provided
        FirstName? firstName = !string.IsNullOrWhiteSpace(request.FirstName)
            ? new FirstName(request.FirstName)
            : null;

        LastName? lastName = !string.IsNullOrWhiteSpace(request.LastName)
            ? new LastName(request.LastName)
            : null;

        Email? email = !string.IsNullOrWhiteSpace(request.Email)
            ? new Email(request.Email)
            : null;

        PhoneNumber? phoneNumber = !string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? new PhoneNumber(request.PhoneNumber)
            : null;

        Address? address = null;
        if (!string.IsNullOrWhiteSpace(request.Country) ||
            !string.IsNullOrWhiteSpace(request.City) ||
            !string.IsNullOrWhiteSpace(request.ZipCode) ||
            !string.IsNullOrWhiteSpace(request.Street))
        {
            // If any address field is provided, all must be provided for a complete address
            if (string.IsNullOrWhiteSpace(request.Country) ||
                string.IsNullOrWhiteSpace(request.City) ||
                string.IsNullOrWhiteSpace(request.ZipCode) ||
                string.IsNullOrWhiteSpace(request.Street))
            {
                return Result.Failure(AddressErrors.AddressIncomplete);
            }

            address = new Address(
                request.Country,
                request.City,
                request.ZipCode,
                request.Street);
        }

        // Update user in Keycloak if email, firstName or lastName changed
        if (!string.IsNullOrEmpty(user.IdentityId) &&
            (email is not null || firstName is not null || lastName is not null))
        {
            try
            {
                await _authenticationService.UpdateUserAsync(
                    user.IdentityId,
                    email?.Value ?? user.Email.Value,
                    firstName?.Value ?? user.FirstName.Value,
                    lastName?.Value ?? user.LastName.Value,
                    cancellationToken);
            }
            catch (Exception)
            {
                return Result.Failure(UserErrors.UpdateFailed);
            }
        }

        // Update domain entity
        Result updateResult = user.Update(
            firstName,
            lastName,
            email,
            phoneNumber,
            request.Gender,
            address);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
