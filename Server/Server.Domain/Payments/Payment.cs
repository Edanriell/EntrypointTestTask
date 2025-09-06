using Server.Domain.Abstractions;
using Server.Domain.Payments.Events;
using Server.Domain.Refunds;
using Server.Domain.Shared;

namespace Server.Domain.Payments;

public sealed class Payment : Entity
{
    // private readonly List<Refund> _refunds = new();

    private Payment(
        Guid id,
        Guid orderId,
        Money amount,
        PaymentStatus paymentStatus,
        PaymentMethod paymentMethod) : base(id)
    {
        OrderId = orderId;
        Amount = amount;
        PaymentStatus = paymentStatus;
        PaymentMethod = paymentMethod;
        CreatedAt = DateTime.UtcNow;
    }

    private Payment() { }

    public Guid OrderId { get; }
    public Money Amount { get; }
    public PaymentStatus PaymentStatus { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? PaymentCompletedAt { get; private set; }
    public DateTime? PaymentFailedAt { get; private set; }
    public PaymentFailureReason? PaymentFailureReason { get; private set; }

    public DateTime? PaymentExpiredAt { get; private set; }

    // public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();
    public Refund? Refund { get; private set; }

    public static Result<Payment> Create(
        Guid orderId,
        Money amount,
        PaymentMethod paymentMethod)
    {
        if (amount <= Money.Zero())
        {
            return Result.Failure<Payment>(PaymentErrors.InvalidPaymentAmount);
        }

        var payment = new Payment(
            Guid.NewGuid(),
            orderId,
            amount,
            PaymentStatus.Pending,
            paymentMethod);

        return Result.Success(payment);
    }

    public Result Process()
    {
        if (PaymentStatus != PaymentStatus.Pending)
        {
            return Result.Failure(PaymentErrors.PaymentAlreadyProcessed);
        }

        if (IsExpired())
        {
            PaymentStatus = PaymentStatus.Expired;
            PaymentFailedAt = DateTime.UtcNow;
            PaymentFailureReason = PaymentFailureReason.Expired;

            RaiseDomainEvent(new PaymentExpiredDomainEvent(Id, OrderId, Amount));

            return Result.Success();
        }

        PaymentStatus = PaymentStatus.Processing;

        // Simulate processing time (1-5 seconds)
        int processingTimeMs = Random.Shared.Next(1000, 5001);
        Thread.Sleep(processingTimeMs);

        // 85% chance of success, 15% chance of failure
        double random = Random.Shared.NextDouble();
        bool isSuccess = random <= 0.85;

        if (isSuccess)
        {
            PaymentStatus = PaymentStatus.Paid;
            PaymentCompletedAt = DateTime.UtcNow;
            RaiseDomainEvent(new PaymentProcessedDomainEvent(Id, OrderId, Amount));
        }
        else
        {
            PaymentStatus = PaymentStatus.Failed;
            PaymentFailedAt = DateTime.UtcNow;
            PaymentFailureReason = PaymentFailureReason.GetRandomReason();
            RaiseDomainEvent(new PaymentFailedDomainEvent(Id, OrderId, Amount, PaymentFailureReason));
        }

        return Result.Success();
    }

    public Result Cancel()
    {
        if (PaymentStatus is not PaymentStatus.Pending)
        {
            return Result.Failure(PaymentErrors.CannotCancelProcessedPayment);
        }

        PaymentStatus = PaymentStatus.Cancelled;
        PaymentFailedAt = DateTime.UtcNow;
        PaymentFailureReason = PaymentFailureReason.Cancelled;

        RaiseDomainEvent(new PaymentCancelledDomainEvent(Id, OrderId, PaymentFailureReason.Cancelled));

        return Result.Success();
    }

    public Result ProcessRefund(Money refundAmount, RefundReason reason)
    {
        if (PaymentStatus != PaymentStatus.Paid)
        {
            return Result.Failure<Refund>(PaymentErrors.CannotRefundUnpaidPayment);
        }

        if (refundAmount.Amount == Amount.Amount)
        {
            PaymentStatus = PaymentStatus.Refunded;
        }

        RaiseDomainEvent(new PaymentRefundedDomainEvent(Id, OrderId, refundAmount, reason));

        return Result.Success();
    }

    public Money GetRemainingAmount()
    {
        return PaymentStatus switch
        {
            PaymentStatus.Refunded => Money.Zero(Amount.Currency),
            _ => Amount
        };
    }

    public Money GetRefundedAmount()
    {
        if (Refund is null)
        {
            return Money.Zero(Amount.Currency);
        }

        Money refundAmount = Refund.Amount;

        return new Money(refundAmount.Amount, Amount.Currency);
    }

    public bool CanBeRefunded()
    {
        return PaymentStatus == PaymentStatus.Paid;
    }

    public Result RefundPayment(RefundReason reason)
    {
        if (PaymentStatus != PaymentStatus.Paid)
        {
            return Result.Failure(PaymentErrors.CannotRefundUnpaidPayment);
        }

        PaymentStatus = PaymentStatus.Refunded;

        RaiseDomainEvent(new PaymentRefundedDomainEvent(Id, OrderId, Amount, reason));

        return Result.Success();
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow - CreatedAt > TimeSpan.FromDays(3);
    }

    public bool IsCancelled()
    {
        return PaymentStatus == PaymentStatus.Cancelled;
    }

    public bool IsCompleted()
    {
        return PaymentStatus == PaymentStatus.Paid;
    }

    public bool IsFailed()
    {
        return PaymentStatus == PaymentStatus.Failed;
    }

    public bool IsRefunded()
    {
        return PaymentStatus == PaymentStatus.Refunded;
    }

    public Result UpdateStatus(PaymentStatus newStatus)
    {
        if (!IsValidStatusTransition(PaymentStatus, newStatus))
        {
            return Result.Failure(PaymentErrors.InvalidStatusTransition);
        }

        PaymentStatus = newStatus;
        return Result.Success();
    }

    private bool IsValidStatusTransition(PaymentStatus from, PaymentStatus to)
    {
        return from switch
        {
            PaymentStatus.Pending => to is PaymentStatus.Processing or PaymentStatus.Paid or PaymentStatus.Failed
                or PaymentStatus.Cancelled or PaymentStatus.Expired,
            PaymentStatus.Processing => to is PaymentStatus.Paid or PaymentStatus.Failed or PaymentStatus.Cancelled,
            _ => false
        };
    }
}
