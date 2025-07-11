using Server.Application.Abstractions.Authentication;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Shared;
using Server.Domain.Users;

namespace Server.Application.Users.RegisterCustomer;

internal sealed class RegisterCustomerCommandHandler : ICommandHandler<RegisterCustomerCommand, Guid>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public RegisterCustomerCommandHandler(
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        RegisterCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var user = User.CreateCustomer(
            new FirstName(
                request.FirstName
            ),
            new LastName(
                request.LastName
            ),
            new Email(
                request.Email
            ),
            new PhoneNumber(
                request.PhoneNumber),
            request.Gender,
            new Address(
                request.Country,
                request.City,
                request.ZipCode,
                request.Street)
        );

        string identityId = await _authenticationService.RegisterAsync(
            user,
            request.Password,
            cancellationToken
        );

        user.SetIdentityId(identityId);

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken
        );

        return user.Id;
    }
}
