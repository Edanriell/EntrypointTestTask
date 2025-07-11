using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Shared;
using Server.Domain.Users.Events;

namespace Server.Domain.Users;

public sealed class User : Entity
{
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

    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    // Navigation property
    public List<Order>? Orders { get; private set; }

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

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        List<Role> rolesList = roles?.ToList() ?? new List<Role>();

        if (rolesList.Count > 0)
        {
            user._roles.AddRange(rolesList);
        }
        else
        {
            user._roles.Add(Role.Guest);
        }

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

    public Result Update(
        FirstName? firstName, LastName? lastName, Email? email, PhoneNumber? phoneNumber, Gender? gender,
        Address? address)
    {
        if (firstName is not null && !FirstName.Equals(firstName))
        {
            FirstName = firstName;
        }

        if (lastName is not null && !LastName.Equals(lastName))
        {
            LastName = lastName;
        }

        if (email is not null && !Email.Equals(email))
        {
            Email = email;

            RaiseDomainEvent(new UserEmailChangedDomainEvent(Id, Email, email));
        }

        if (phoneNumber is not null && !PhoneNumber.Equals(phoneNumber))
        {
            PhoneNumber = phoneNumber;

            RaiseDomainEvent(new UserPhoneNumberChangedDomainEvent(Id, PhoneNumber, phoneNumber));
        }

        if (gender is not null && !Gender.Equals(gender))
        {
            Gender = gender.Value;
        }

        if (address is not null && !Address.Equals(address))
        {
            Address = address;

            RaiseDomainEvent(new UserAddressChangedDomainEvent(Id, Address, address));
        }

        RaiseDomainEvent(new UserUpdatedDomainEvent(Id));

        return Result.Success();
    }

    public Result Delete()
    {
        if (Orders?.Any(order => order.Status != OrderStatus.Delivered &&
            order.Status != OrderStatus.Cancelled &&
            order.Status != OrderStatus.Returned) == true)
        {
            return Result.Failure(UserErrors.CannotDeleteUserWithActiveOrders);
        }

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
