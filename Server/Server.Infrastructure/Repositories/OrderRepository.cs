using Bookify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Orders;
using Server.Domain.Payments;

namespace Server.Infrastructure.Repositories;

internal sealed class OrderRepository
    : Repository<Order>,
        IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .Include(o => o.Payments)
            .ThenInclude(p => p.Refunds)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public void Add(Order order) { DbContext.Add(order); }

    public void Update(Order order) { DbContext.Update(order); }

    public void Remove(Order order) { DbContext.Remove(order); }

    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .Include(o => o.Payments)
            .ThenInclude(p => p.Refunds)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.ClientId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetByStatusAsync(
        OrderStatus status, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .Include(o => o.Payments)
            .ThenInclude(p => p.Refunds)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersWithPendingPaymentsAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .Include(o => o.Payments)
            .ThenInclude(p => p.Refunds)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.Payments.Any(p => p.PaymentStatus == PaymentStatus.Pending))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersWithFailedPaymentsAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .Include(o => o.Payments)
            .ThenInclude(p => p.Refunds)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.Payments.Any(p => p.PaymentStatus == PaymentStatus.Failed))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetRecentOrdersAsync(
        int count, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .Include(o => o.Payments)
            .ThenInclude(p => p.Refunds)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
