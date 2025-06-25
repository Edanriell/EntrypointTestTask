using Server.Domain.Abstractions;

namespace Server.Domain.Users;

public static class UserErrors
{
    public static readonly Error CannotDeleteUserWithActiveOrders = new(
        "User.CannotDeleteWithActiveOrders",
        "Cannot delete user that has active orders");

    public static readonly Error NotFound = new(
        "User.Found",
        "The user with the specified identifier was not found"
    );

    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid"
    );

    public static readonly Error UpdateFailed = new(
        "User.UpdateFailed",
        "Failed to update user in authentication service"
    );

    public static readonly Error DeleteFailed = new(
        "User.DeleteFailed",
        "Failed to delete user from authentication service"
    );
}
