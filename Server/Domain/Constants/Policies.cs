namespace Domain.Constants;

public abstract class Policies
{
    public const string CanManageUserAccount = nameof(CanManageUserAccount);
    public const string CanManageUsers = nameof(CanManageUsers);
    public const string CanChangeUserRole = nameof(CanChangeUserRole);
    public const string CanManageOrders = nameof(CanManageOrders);
    public const string CanDeleteOrders = nameof(CanDeleteOrders);
    public const string CanGetOrder = nameof(CanGetOrder);
    public const string CanCreateOrder = nameof(CanCreateOrder);
    public const string CanManageProducts = nameof(CanManageProducts);
    public const string CanDeleteProducts = nameof(CanDeleteProducts);
    public const string CanAccessAdminPanel = nameof(CanAccessAdminPanel);
}