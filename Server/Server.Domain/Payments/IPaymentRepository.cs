namespace Server.Domain.Payments;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    void Add(Payment payment);
    void Update(Payment payment);
    void Remove(Payment payment);
}
