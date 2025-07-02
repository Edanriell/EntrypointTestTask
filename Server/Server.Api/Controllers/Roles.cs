namespace Server.Api.Controllers;

// Mirror's Realm roles in keycloak 
internal static class Roles
{
    public const string Admin = "Admin";

    // Client
    public const string Customer = "Customer";
    public const string Guest = "Guest";
    public const string Manager = "Manager";
    public const string OrderManager = "OrderManager";
    public const string PaymentManager = "PaymentManager";
    public const string ProductManager = "ProductManager";
    public const string UserManager = "UserManager";
}
