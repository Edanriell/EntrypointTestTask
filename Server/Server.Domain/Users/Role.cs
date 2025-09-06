namespace Server.Domain.Users;

public sealed class Role
{
    public static readonly Role Admin = new(1, "Admin");
    public static readonly Role Manager = new(2, "Manager");
    public static readonly Role OrderManager = new(3, "OrderManager");
    public static readonly Role PaymentManager = new(4, "PaymentManager");
    public static readonly Role ProductManager = new(5, "ProductManager");
    public static readonly Role UserManager = new(6, "UserManager");
    public static readonly Role Customer = new(7, "Customer");
    public static readonly Role Guest = new(8, "Guest");

    private static readonly Role[] AllRoles =
    {
        Admin, Manager, OrderManager, PaymentManager,
        ProductManager, UserManager, Customer, Guest
    };
 
    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; }

    public ICollection<User> Users { get; init; } = new List<User>();

    public ICollection<Permission> Permissions { get; init; } = new List<Permission>();

    public static IEnumerable<Role> FromNames(IEnumerable<string> roleNames)
    {
        return roleNames
            .Select(roleName => AllRoles.FirstOrDefault(r => r.Name == roleName))
            .Where(role => role != null)
            .Cast<Role>();
    }
}
 
