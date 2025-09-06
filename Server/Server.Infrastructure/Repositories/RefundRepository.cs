using Server.Domain.Refunds;

namespace Server.Infrastructure.Repositories;

internal sealed class RefundRepository
    : Repository<Refund>,
        IRefundRepository
{
    public RefundRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public void Add(Refund refund) { DbContext.Add(refund); }
    public void Update(Refund refund) { DbContext.Update(refund); }
    public void Remove(Refund refund) { DbContext.Remove(refund); }
}
