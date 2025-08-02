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
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByIdNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .AsNoTracking()
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
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersWithPendingPaymentsAsync(
        CancellationToken cancellationToken = default)
    {
        // ✅ Use JOIN to get orders with pending payments
        List<Guid> orderIds = await DbContext
            .Set<Payment>()
            .Where(p => p.PaymentStatus == PaymentStatus.Pending)
            .Select(p => p.OrderId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await DbContext
            .Set<Order>()
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => orderIds.Contains(o.Id))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersWithFailedPaymentsAsync(
        CancellationToken cancellationToken = default)
    {
        // ✅ Use JOIN to get orders with failed payments
        List<Guid> orderIds = await DbContext
            .Set<Payment>()
            .Where(p => p.PaymentStatus == PaymentStatus.Failed)
            .Select(p => p.OrderId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await DbContext
            .Set<Order>()
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => orderIds.Contains(o.Id))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetRecentOrdersAsync(
        int count, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetActiveOrdersByUserIdAsync(
        Guid clientId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Order>()
            .Where(o => o.ClientId == clientId &&
                o.Status != OrderStatus.Delivered &&
                o.Status != OrderStatus.Cancelled)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Order>()
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}

// internal sealed class OrderRepository
//     : Repository<Order>,
//         IOrderRepository
// {
//     public OrderRepository(ApplicationDbContext dbContext) : base(dbContext) { }
//
//     public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
//     {
//         return await DbContext
//             .Set<Order>()
//             .Include(o => o.Payments)
//             .ThenInclude(p => p.Refunds)
//             .Include(o => o.OrderProducts)
//             .ThenInclude(op => op.Product)
//             .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
//     }
//
//     public void Add(Order order) { DbContext.Add(order); }
//
//     public void Update(Order order) { DbContext.Update(order); }
//
//     public void Remove(Order order) { DbContext.Remove(order); }
//
//     public async Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
//     {
//         return await DbContext
//             .Set<Order>()
//             .Include(o => o.Payments)
//             .ThenInclude(p => p.Refunds)
//             .Include(o => o.OrderProducts)
//             .ThenInclude(op => op.Product)
//             .Where(o => o.ClientId == userId)
//             .OrderByDescending(o => o.CreatedAt)
//             .ToListAsync(cancellationToken);
//     }
//
//     public async Task<IReadOnlyList<Order>> GetByStatusAsync(
//         OrderStatus status, CancellationToken cancellationToken = default)
//     {
//         return await DbContext
//             .Set<Order>()
//             .Include(o => o.Payments)
//             .ThenInclude(p => p.Refunds)
//             .Include(o => o.OrderProducts)
//             .ThenInclude(op => op.Product)
//             .Where(o => o.Status == status)
//             .OrderByDescending(o => o.CreatedAt)
//             .ToListAsync(cancellationToken);
//     }
//
//     public async Task<IReadOnlyList<Order>> GetOrdersWithPendingPaymentsAsync(
//         CancellationToken cancellationToken = default)
//     {
//         return await DbContext
//             .Set<Order>()
//             .Include(o => o.Payments)
//             .ThenInclude(p => p.Refunds)
//             .Include(o => o.OrderProducts)
//             .ThenInclude(op => op.Product)
//             .Where(o => o.Payments.Any(p => p.PaymentStatus == PaymentStatus.Pending))
//             .OrderByDescending(o => o.CreatedAt)
//             .ToListAsync(cancellationToken);
//     }
//
//     public async Task<IReadOnlyList<Order>> GetOrdersWithFailedPaymentsAsync(
//         CancellationToken cancellationToken = default)
//     {
//         return await DbContext
//             .Set<Order>()
//             .Include(o => o.Payments)
//             .ThenInclude(p => p.Refunds)
//             .Include(o => o.OrderProducts)
//             .ThenInclude(op => op.Product)
//             .Where(o => o.Payments.Any(p => p.PaymentStatus == PaymentStatus.Failed))
//             .OrderByDescending(o => o.CreatedAt)
//             .ToListAsync(cancellationToken);
//     }
//
//     public async Task<IReadOnlyList<Order>> GetRecentOrdersAsync(
//         int count, CancellationToken cancellationToken = default)
//     {
//         return await DbContext
//             .Set<Order>()
//             .Include(o => o.Payments)
//             .ThenInclude(p => p.Refunds)
//             .Include(o => o.OrderProducts)
//             .ThenInclude(op => op.Product)
//             .OrderByDescending(o => o.CreatedAt)
//             .Take(count)
//             .ToListAsync(cancellationToken);
//     }
//
//     public async Task<IEnumerable<Order>> GetActiveOrdersByUserIdAsync(
//         Guid clientId, CancellationToken cancellationToken = default)
//     {
//         return await DbContext.Set<Order>()
//             .Where(o => o.ClientId == clientId &&
//                 o.Status != OrderStatus.Delivered &&
//                 o.Status != OrderStatus.Cancelled)
//             .ToListAsync(cancellationToken);
//     }
// }
