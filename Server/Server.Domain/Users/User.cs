using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Shared;
using Server.Domain.Users.Events;

namespace Server.Domain.Users;

public sealed class User : Entity
{
    private readonly List<Order> _orders = new();
    private readonly List<Role> _roles = new();

    private User(
        Guid id, FirstName firstName, LastName lastName, Email email, PhoneNumber phoneNumber, Gender gender,
        Address address, DateTime createdAt) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Gender = gender;
        Address = address;
        CreatedAt = createdAt;
    }
 
    private User() { }

    public string IdentityId { get; private set; } = string.Empty;
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Gender Gender { get; private set; }
    public Address Address { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    public static User CreateUser(
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber phoneNumber,
        Gender gender,
        Address address,
        IEnumerable<Role>? roles = null)
    {
        var user = new User(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            phoneNumber,
            gender,
            address,
            DateTime.UtcNow
        );

        List<Role> rolesList = roles?.ToList() ?? new List<Role>();
        user._roles.AddRange(rolesList.Any() ? rolesList : [Role.Guest]);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        return user;
    }

    public static User CreateCustomer(
        FirstName firstName, LastName lastName, Email email, PhoneNumber phoneNumber, Gender gender, Address address)
    {
        var user = new User(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            phoneNumber,
            gender,
            address,
            DateTime.UtcNow
        );

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        user._roles.Add(
            Role.Customer
        );

        return user;
    }

    public Result ChangeEmail(Email newEmail)
    {
        if (Email.Equals(newEmail))
        {
            return Result.Success();
        }

        Email oldEmail = Email;
        Email = newEmail;

        RaiseDomainEvent(new UserEmailChangedDomainEvent(Id, oldEmail, newEmail));

        return Result.Success();
    }

    public Result ChangePhoneNumber(PhoneNumber newPhoneNumber)
    {
        if (PhoneNumber.Equals(newPhoneNumber))
        {
            return Result.Success();
        }

        PhoneNumber oldPhoneNumber = PhoneNumber;
        PhoneNumber = newPhoneNumber;

        RaiseDomainEvent(new UserPhoneNumberChangedDomainEvent(Id, oldPhoneNumber, newPhoneNumber));

        return Result.Success();
    }

    public Result UpdatePersonalInfo(FirstName? firstName, LastName? lastName)
    {
        bool hasChanges = false;

        if (!FirstName.Equals(firstName) && firstName is not null)
        {
            FirstName = firstName;
            hasChanges = true;
        }

        if (!LastName.Equals(lastName) && lastName is not null)
        {
            LastName = lastName;
            hasChanges = true;
        }

        if (hasChanges)
        {
            RaiseDomainEvent(new UserPersonalInfoUpdatedDomainEvent(Id, FirstName, LastName));
        }

        return Result.Success();
    }

    public Result ChangeGender(Gender newGender)
    {
        if (Gender.Equals(newGender))
        {
            return Result.Success();
        }

        Gender = newGender;

        return Result.Success();
    }

    public Result ChangeAddress(Address newAddress)
    {
        if (Address.Equals(newAddress))
        {
            return Result.Success();
        }

        Address oldAddress = Address;
        Address = newAddress;

        RaiseDomainEvent(new UserAddressChangedDomainEvent(Id, oldAddress, newAddress));

        return Result.Success();
    }

    public Result Update(
        FirstName? firstName = null,
        LastName? lastName = null,
        Email? email = null,
        PhoneNumber? phoneNumber = null,
        Gender? gender = null,
        Address? address = null)
    {
        if (firstName is not null || lastName is not null)
        {
            Result personalInfoResult = UpdatePersonalInfo(firstName, lastName);
            if (personalInfoResult.IsFailure)
            {
                return Result.Failure(personalInfoResult.Error);
            }
        }

        if (email is not null)
        {
            Result emailResult = ChangeEmail(email);
            if (emailResult.IsFailure)
            {
                return Result.Failure(emailResult.Error);
            }
        }

        if (phoneNumber is not null)
        {
            Result phoneResult = ChangePhoneNumber(phoneNumber);
            if (phoneResult.IsFailure)
            {
                return Result.Failure(phoneResult.Error);
            }
        }

        if (gender is not null)
        {
            Result genderResult = ChangeGender(gender.Value);
            if (genderResult.IsFailure)
            {
                return Result.Failure(genderResult.Error);
            }
        }

        if (address is not null)
        {
            Result addressResult = ChangeAddress(address);
            if (addressResult.IsFailure)
            {
                return Result.Failure(addressResult.Error);
            }
        }

        return Result.Success();
    }

    public Result Delete()
    {
        RaiseDomainEvent(new UserDeletedDomainEvent(Id, Email.Value));

        return Result.Success();
    }

    public Result AddRole(Role role)
    {
        if (_roles.Contains(role))
        {
            return Result.Failure(UserErrors.RoleAlreadyAssigned);
        }

        _roles.Add(role);

        RaiseDomainEvent(new UserRoleAddedDomainEvent(Id, role.Id));

        return Result.Success();
    }

    public Result RemoveRole(Role role)
    {
        if (!_roles.Contains(role))
        {
            return Result.Failure(UserErrors.RoleNotAssigned);
        }

        _roles.Remove(role);

        RaiseDomainEvent(new UserRoleRemovedDomainEvent(Id, role.Id));

        return Result.Success();
    }

    public void SetIdentityId(string identityId)
    {
        if (!string.IsNullOrWhiteSpace(IdentityId))
        {
            throw new InvalidOperationException("Identity ID has already been set");
        }

        if (string.IsNullOrWhiteSpace(identityId))
        {
            throw new ArgumentException("Identity ID cannot be null or empty", nameof(identityId));
        }

        IdentityId = identityId;
    }

    public bool HasPermission(Permission permission)
    {
        return _roles.Any(role => role.Permissions.Contains(permission));
    }

    public bool HasAnyPermission(params Permission[] permissions)
    {
        return permissions.Any(HasPermission);
    }

    public bool HasAllPermissions(params Permission[] permissions)
    {
        return permissions.All(HasPermission);
    }

    public IEnumerable<Permission> GetAllPermissions()
    {
        return _roles.SelectMany(role => role.Permissions).Distinct();
    }
}
