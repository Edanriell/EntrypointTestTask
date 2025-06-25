using Bookify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Orders;

namespace Server.Infrastructure.Repositories;

internal sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Order>()
            .Include(o => o.Client)
            .Include(o => o.Payment)
            .Include(o => o.OrderProducts)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Order>()
            .Include(o => o.Client)
            .Include(o => o.Payment)
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public void Add(Order order) { DbContext.Set<Order>().Add(order); }

    public void Update(Order order) { DbContext.Set<Order>().Update(order); }

    public void Remove(Order order) { DbContext.Set<Order>().Remove(order); }
}