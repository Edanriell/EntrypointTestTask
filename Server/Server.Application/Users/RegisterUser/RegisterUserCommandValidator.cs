using FluentValidation;
using Server.Domain.Users;

namespace Server.Application.Users.RegisterUser;
 
internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private static readonly string[] ValidRoleNames =
    {
        Role.Admin.Name,
        Role.Manager.Name,
        Role.OrderManager.Name,
        Role.PaymentManager.Name,
        Role.ProductManager.Name,
        Role.UserManager.Name,
        Role.Customer.Name,
        Role.Guest.Name
    };

    private readonly IUserRepository _userRepository;

    public RegisterUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255)
            .MustAsync(BeUniqueEmail)
            .WithMessage("Email address is already in use");

        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in valid international format")
            .MustAsync(BeUniquePhoneNumber)
            .WithMessage("Phone number is already in use");

        RuleFor(c => c.Gender)
            .IsInEnum();

        RuleFor(c => c.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.ZipCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(c => c.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Must(ContainUppercase)
            .WithMessage("Password must contain at least one uppercase letter")
            .Must(ContainLowercase)
            .WithMessage("Password must contain at least one lowercase letter")
            .Must(ContainDigit)
            .WithMessage("Password must contain at least one digit")
            .Must(ContainSpecialCharacter)
            .WithMessage("Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?)")
            .MaximumLength(128)
            .WithMessage("Password cannot exceed 128 characters");

        RuleFor(x => x.RoleNames)
            .Must(BeValidRoles)
            .WithMessage($"Role names must be one of: {string.Join(", ", ValidRoleNames)}")
            .When(x => x.RoleNames != null);

        RuleForEach(x => x.RoleNames)
            .Must(roleName => ValidRoleNames.Contains(roleName))
            .WithMessage("Each role name must be valid.")
            .When(x => x.RoleNames != null);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        User? existingUser = await _userRepository.GetByEmailAsync(new Email(email), cancellationToken);
        return existingUser is null;
    }

    private async Task<bool> BeUniquePhoneNumber(string phoneNumber, CancellationToken cancellationToken)
    {
        User? existingUser =
            await _userRepository.GetByPhoneNumberAsync(new PhoneNumber(phoneNumber), cancellationToken);
        return existingUser is null;
    }

    private static bool ContainUppercase(string password)
    {
        return password.Any(char.IsUpper);
    }

    private static bool ContainLowercase(string password)
    {
        return password.Any(char.IsLower);
    }

    private static bool ContainDigit(string password)
    {
        return password.Any(char.IsDigit);
    }

    private static bool ContainSpecialCharacter(string password)
    {
        const string specialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        return password.Any(specialCharacters.Contains);
    }

    private static bool BeValidRoles(IEnumerable<string>? roleNames)
    {
        if (roleNames == null)
        {
            return true;
        }

        var roleList = roleNames.ToList();
        return roleList.All(roleName => ValidRoleNames.Contains(roleName)) &&
            roleList.Distinct().Count() == roleList.Count; // No duplicates
    }
}
