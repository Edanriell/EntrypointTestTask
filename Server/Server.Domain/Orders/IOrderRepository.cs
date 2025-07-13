namespace Server.Domain.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersWithPendingPaymentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersWithFailedPaymentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default);
    void Add(Order order);
    void Update(Order order);
    void Remove(Order order);
}
