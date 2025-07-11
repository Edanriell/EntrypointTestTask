using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Shared;

namespace Server.Domain.Payments;

public sealed class Refund : Entity
{
    private Refund(
        Guid id,
        Guid paymentId,
        Money amount,
        RefundReason reason,
        DateTime createdAt) : base(id)
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
        CreatedAt = createdAt;
        Status = RefundStatus.Pending;
    }

    private Refund() { }

    public Guid PaymentId { get; }
    public Money Amount { get; }
    public RefundReason Reason { get; }
    public RefundStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? ProcessedAt { get; private set; }
    public string? RefundReference { get; private set; }

    // Navigation property
    public Payment Payment { get; private set; }

    public static Result<Refund> Create(
        Guid paymentId,
        Money amount,
        RefundReason reason)
    {
        if (amount <= Money.Zero())
        {
            return Result.Failure<Refund>(PaymentErrors.InvalidRefundAmount);
        }

        var refund = new Refund(
            Guid.NewGuid(),
            paymentId,
            amount,
            reason,
            DateTime.UtcNow);

        return Result.Success(refund);
    }

    public Result ProcessRefund(string? refundReference = null)
    {
        if (Status != RefundStatus.Pending)
        {
            return Result.Failure(PaymentErrors.RefundAlreadyProcessed);
        }

        Status = RefundStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        RefundReference = refundReference;

        return Result.Success();
    }

    public Result FailRefund(string reason)
    {
        if (Status != RefundStatus.Pending)
        {
            return Result.Failure(PaymentErrors.RefundAlreadyProcessed);
        }

        Status = RefundStatus.Failed;
        ProcessedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
