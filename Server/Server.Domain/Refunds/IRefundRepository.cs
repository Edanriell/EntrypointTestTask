namespace Server.Domain.Refunds;

public interface IRefundRepository
{
    void Add(Refund refund);
    void Update(Refund refund);
    void Remove(Refund refund);
}
