using Microsoft.EntityFrameworkCore;
using Server.Domain.Payments;

namespace Server.Infrastructure.Repositories;

internal sealed class PaymentRepository
    : Repository<Payment>,
        IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Refund)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByOrderIdAsync(
        Guid orderId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Refund)
            .Where(p => p.OrderId == orderId)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByStatusAsync(
        PaymentStatus status, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Refund)
            .Where(p => p.PaymentStatus == status)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByPaymentMethodAsync(
        PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Refund)
            .Where(p => p.PaymentMethod == paymentMethod)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetPendingPaymentsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Refund)
            .Where(p => p.PaymentStatus == PaymentStatus.Pending)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetExpiredPaymentsAsync(
        DateTime expirationDate, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Refund)
            .Where(p => p.PaymentStatus == PaymentStatus.Pending && p.CreatedAt < expirationDate)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public void Add(Payment payment) { DbContext.Add(payment); }

    public void Update(Payment payment) { DbContext.Update(payment); }

    public void Remove(Payment payment) { DbContext.Remove(payment); }
}
