using FluentValidation;
using Server.Domain.Users;

namespace Server.Application.Users.UpdateUser;

internal sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(c => c.FirstName)
            .MaximumLength(50)
            .When(c => !string.IsNullOrWhiteSpace(c.FirstName))
            .WithMessage("First name cannot exceed 50 characters");

        RuleFor(c => c.LastName)
            .MaximumLength(50)
            .When(c => !string.IsNullOrWhiteSpace(c.LastName))
            .WithMessage("Last name cannot exceed 50 characters");

        RuleFor(c => c.Email)
            .EmailAddress()
            .MaximumLength(255)
            .When(c => !string.IsNullOrWhiteSpace(c.Email))
            .WithMessage("Invalid email format or email exceeds 255 characters")
            .MustAsync(BeUniqueEmail)
            .When(c => !string.IsNullOrWhiteSpace(c.Email))
            .WithMessage("Email address is already in use");

        RuleFor(c => c.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(c => !string.IsNullOrWhiteSpace(c.PhoneNumber))
            .WithMessage("Phone number must be in valid international format")
            .MustAsync(BeUniquePhoneNumber)
            .When(c => !string.IsNullOrWhiteSpace(c.PhoneNumber))
            .WithMessage("Phone number is already in use");

        RuleFor(c => c.Gender)
            .IsInEnum()
            .When(c => c.Gender.HasValue)
            .WithMessage("Invalid gender value");

        RuleFor(c => c.Country)
            .MaximumLength(100)
            .When(c => !string.IsNullOrWhiteSpace(c.Country))
            .WithMessage("Country cannot exceed 100 characters");

        RuleFor(c => c.City)
            .MaximumLength(100)
            .When(c => !string.IsNullOrWhiteSpace(c.City))
            .WithMessage("City cannot exceed 100 characters");

        RuleFor(c => c.ZipCode)
            .MaximumLength(20)
            .When(c => !string.IsNullOrWhiteSpace(c.ZipCode))
            .WithMessage("Zip code cannot exceed 20 characters");

        RuleFor(c => c.Street)
            .MaximumLength(200)
            .When(c => !string.IsNullOrWhiteSpace(c.Street))
            .WithMessage("Street cannot exceed 200 characters");

        RuleFor(c => c)
            .Must(HaveCompleteAddressOrNone)
            .WithMessage(
                "All address fields (Country, City, ZipCode, Street) must be provided together when updating address")
            .When(c => HasAnyAddressField(c));
    }

    private static bool HasAnyAddressField(UpdateUserCommand command)
    {
        return !string.IsNullOrWhiteSpace(command.Country) ||
            !string.IsNullOrWhiteSpace(command.City) ||
            !string.IsNullOrWhiteSpace(command.ZipCode) ||
            !string.IsNullOrWhiteSpace(command.Street);
    }

    private static bool HaveCompleteAddressOrNone(UpdateUserCommand command)
    {
        bool hasCountry = !string.IsNullOrWhiteSpace(command.Country);
        bool hasCity = !string.IsNullOrWhiteSpace(command.City);
        bool hasZipCode = !string.IsNullOrWhiteSpace(command.ZipCode);
        bool hasStreet = !string.IsNullOrWhiteSpace(command.Street);

        bool allPresent = hasCountry && hasCity && hasZipCode && hasStreet;
        bool allMissing = !hasCountry && !hasCity && !hasZipCode && !hasStreet;

        return allPresent || allMissing;
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
}
 
