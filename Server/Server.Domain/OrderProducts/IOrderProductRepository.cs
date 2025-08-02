namespace Server.Domain.OrderProducts;

public interface IOrderProductRepository
{
    void Add(OrderProduct orderProduct);
    void AddRange(IEnumerable<OrderProduct> orderProducts);
    Task<OrderProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<OrderProduct>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    void Remove(OrderProduct orderProduct);
    void Update(OrderProduct orderProduct);
}
