namespace Server.Domain.Orders;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(Order order);
    void Update(Order order);
    void Remove(Order order);
}
