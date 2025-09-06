using Server.Domain.Abstractions;
using Server.Domain.Orders;

namespace Server.Domain.Users;

public sealed class UserService
{
    private readonly IOrderRepository _orderRepository;

    public UserService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result> DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        IEnumerable<Order> activeOrders =
            await _orderRepository.GetActiveOrdersByUserIdAsync(user.Id, cancellationToken);

        if (activeOrders.Any())
        {
            return Result.Failure(UserErrors.CannotDeleteUserWithActiveOrders);
        }

        return user.Delete();
    }
}
 
