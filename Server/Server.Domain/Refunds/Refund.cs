using Server.Domain.Abstractions;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Domain.Refunds;

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
        Status = RefundStatus.Processed;
    }

    private Refund() { }

    public Guid PaymentId { get; private set; }
    public Money Amount { get; private set; }
    public RefundReason Reason { get; private set; }
    public RefundStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? RefundReference { get; private set; }
    public string? RefundFailureReason { get; private set; }

    // Navigation property
    public Payment Payment { get; private set; }

    public static Result<Refund> Create(
        Guid paymentId,
        decimal amount,
        RefundReason reason)
    {
        var newRefund = new Money(amount, Currency.Usd);

        var refund = new Refund(
            Guid.NewGuid(),
            paymentId,
            newRefund,
            reason,
            DateTime.UtcNow);

        return Result.Success(refund);
    }

    public void AttachToPayment(Payment payment)
    {
        Payment = payment;
        PaymentId = payment.Id;
    }

    // public Result ProcessRefund(string? refundReference = null)
    // {
    //     if (Status != RefundStatus.Pending)
    //     {
    //         return Result.Failure(PaymentErrors.RefundAlreadyProcessed);
    //     }
    //
    //     Status = RefundStatus.Processed;
    //     ProcessedAt = DateTime.UtcNow;
    //     RefundReference = refundReference;
    //
    //     return Result.Success();
    // }
    //
    // public Result FailRefund(string reason)
    // {
    //     if (Status != RefundStatus.Pending)
    //     {
    //         return Result.Failure(PaymentErrors.RefundAlreadyProcessed);
    //     }
    //
    //     RefundFailureReason = reason;
    //     Status = RefundStatus.Failed;
    //     ProcessedAt = DateTime.UtcNow;
    //
    //     return Result.Success();
    // }
}
