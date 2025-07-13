namespace Server.Domain.Payments;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetByPaymentMethodAsync(
        PaymentMethod paymentMethod, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetPendingPaymentsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetExpiredPaymentsAsync(
        DateTime expirationDate, CancellationToken cancellationToken = default);

    void Add(Payment payment);
    void Update(Payment payment);
    void Remove(Payment payment);
}
