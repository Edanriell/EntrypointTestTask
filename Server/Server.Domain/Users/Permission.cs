namespace Server.Domain.Users;

public sealed class Permission
{
    // User Management
    public static readonly Permission UsersRead = new(1, "users:read");
    public static readonly Permission UsersWrite = new(2, "users:write");
    public static readonly Permission UsersDelete = new(3, "users:delete");
    public static readonly Permission UsersManage = new(4, "users:manage");

    // Client Management
    public static readonly Permission ClientsRead = new(5, "clients:read");
    public static readonly Permission ClientsWrite = new(6, "clients:write");
    public static readonly Permission ClientsDelete = new(7, "clients:delete");
    public static readonly Permission ClientsManage = new(8, "clients:manage");

    // Product Management
    public static readonly Permission ProductsRead = new(9, "products:read");
    public static readonly Permission ProductsWrite = new(10, "products:write");
    public static readonly Permission ProductsDelete = new(11, "products:delete");
    public static readonly Permission ProductsManage = new(12, "products:manage");

    // Order Management
    public static readonly Permission OrdersRead = new(13, "orders:read");
    public static readonly Permission OrdersWrite = new(14, "orders:write");
    public static readonly Permission OrdersDelete = new(15, "orders:delete");
    public static readonly Permission OrdersProcess = new(16, "orders:process");
    public static readonly Permission OrdersManage = new(17, "orders:manage");

    // Payment Management
    public static readonly Permission PaymentsRead = new(18, "payments:read");
    public static readonly Permission PaymentsWrite = new(19, "payments:write");
    public static readonly Permission PaymentsRefund = new(20, "payments:refund");
    public static readonly Permission PaymentsManage = new(21, "payments:manage");

    private Permission(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; }
}
 
