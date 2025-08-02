using Bookify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Server.Domain.OrderProducts;

namespace Server.Infrastructure.Repositories;

internal sealed class OrderProductRepository
    : Repository<OrderProduct>,
        IOrderProductRepository
{
    public OrderProductRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public void Add(OrderProduct orderProduct)
    {
        DbContext.Add(orderProduct);
    }

    public void AddRange(IEnumerable<OrderProduct> orderProducts)
    {
        DbContext.AddRange(orderProducts);
    }

    public async Task<OrderProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<OrderProduct>()
            .Include(op => op.Product)
            .Include(op => op.Order)
            .FirstOrDefaultAsync(op => op.Id == id, cancellationToken);
    }

    public async Task<List<OrderProduct>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<OrderProduct>()
            .Include(op => op.Product)
            .Where(op => op.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public void Remove(OrderProduct orderProduct)
    {
        DbContext.Remove(orderProduct);
    }

    public void Update(OrderProduct orderProduct)
    {
        DbContext.Update(orderProduct);
    }
}
