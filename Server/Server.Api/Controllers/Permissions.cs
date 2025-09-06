namespace Server.Api.Controllers;
 
// Mirror's Realm roles > Associated roles (permissions) in keycloak 
internal static class Permissions
{
    // User Management
    public const string UsersRead = "users:read";
    public const string UsersWrite = "users:write";
    public const string UsersDelete = "users:delete";
    public const string UsersManage = "users:manage";

    // Client Management  
    public const string ClientsRead = "clients:read";
    public const string ClientsWrite = "clients:write";
    public const string ClientsDelete = "clients:delete";
    public const string ClientsManage = "clients:manage";

    // Product Management
    public const string ProductsRead = "products:read";
    public const string ProductsWrite = "products:write";
    public const string ProductsDelete = "products:delete";
    public const string ProductsManage = "products:manage";

    // Order Management
    public const string OrdersRead = "orders:read";
    public const string OrdersWrite = "orders:write";
    public const string OrdersDelete = "orders:delete";
    public const string OrdersProcess = "orders:process";
    public const string OrdersManage = "orders:manage";

    // Payment Management
    public const string PaymentsRead = "payments:read";
    public const string PaymentsWrite = "payments:write";
    public const string PaymentsRefund = "payments:refund";
    public const string PaymentsManage = "payments:manage";
}
 
