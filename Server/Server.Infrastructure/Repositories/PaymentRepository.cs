using Bookify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Payments;

namespace Server.Infrastructure.Repositories;

internal sealed class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Order)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Payment>()
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
    }

    public void Add(Payment payment) { DbContext.Add(payment); }

    public void Update(Payment payment) { DbContext.Update(payment); }

    public void Remove(Payment payment) { DbContext.Remove(payment); }
}