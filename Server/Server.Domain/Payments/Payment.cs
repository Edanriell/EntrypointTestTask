using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments.Events;
using Server.Domain.Shared;

namespace Server.Domain.Payments;

public sealed class Payment : Entity
{
    // MANY PAYMENTS
    // private Payment(
    //     Guid id, Guid orderId, Money totalAmount, PaymentStatus paymentStatus, Money paidAmount,
    //     Money outstandingAmount) : base(id)
    // {
    //     OrderId = orderId;
    //     TotalAmount = totalAmount;
    //     PaymentStatus = paymentStatus;
    //     PaidAmount = paidAmount;
    //     OutstandingAmount = outstandingAmount;
    // }
    private readonly List<Refund> _refunds = new();

    private Payment(
        Guid id,
        Guid orderId,
        Money amount,
        PaymentStatus paymentStatus,
        PaymentMethod paymentMethod,
        string? paymentReference = null) : base(id)
    {
        OrderId = orderId;
        Amount = amount;
        PaymentStatus = paymentStatus;
        PaymentMethod = paymentMethod;
        PaymentReference = paymentReference;
        CreatedAt = DateTime.UtcNow;
    }

    // We must add that of type constructors for EF
    private Payment() { }

    // MANY PAYMENTS
    // public Guid OrderId { get; }
    // public Money TotalAmount { get; private set; }
    // public PaymentStatus PaymentStatus { get; private set; }
    // public Money PaidAmount { get; private set; }
    // public Money OutstandingAmount { get; private set; }
    // public DateTime? PaymentCompletedAt { get; private set; }
    public Guid OrderId { get; }
    public Money Amount { get; }
    public PaymentStatus PaymentStatus { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string? PaymentReference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaymentCompletedAt { get; private set; }

    // Navigation property
    public Order Order { get; private set; }
    public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();

    // MANY PAYMENTS
    public static Result<Payment> Create(
        Guid orderId,
        Money amount,
        PaymentMethod paymentMethod,
        string? paymentReference = null)
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
            paymentMethod,
            paymentReference);

        return Result.Success(payment);
    }

    public Result ProcessPayment()
    {
        if (PaymentStatus != PaymentStatus.Pending)
        {
            return Result.Failure(PaymentErrors.PaymentAlreadyProcessed);
        }

        PaymentStatus = PaymentStatus.Paid;
        PaymentCompletedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentProcessedDomainEvent(Id, OrderId, Amount));

        return Result.Success();
    }


    public Result<Refund> ProcessRefund(Money refundAmount, RefundReason reason)
    {
        if (PaymentStatus != PaymentStatus.Paid && PaymentStatus != PaymentStatus.PartiallyRefunded)
        {
            return Result.Failure<Refund>(PaymentErrors.CannotRefundUnpaidPayment);
        }

        Money totalRefunded = GetTotalRefundedAmount();
        var availableForRefund = new Money(Amount.Amount - totalRefunded.Amount, Amount.Currency);

        if (refundAmount > availableForRefund)
        {
            return Result.Failure<Refund>(PaymentErrors.RefundAmountExceedsPaidAmount);
        }

        if (refundAmount <= Money.Zero())
        {
            return Result.Failure<Refund>(PaymentErrors.InvalidRefundAmount);
        }

        // Create a refund record
        Result<Refund> refundResult = Refund.Create(Id, refundAmount, reason);
        if (refundResult.IsFailure)
        {
            return refundResult;
        }

        _refunds.Add(refundResult.Value);

        // Update payment status
        var newTotalRefunded = new Money(totalRefunded.Amount + refundAmount.Amount, Amount.Currency);
        PaymentStatus = newTotalRefunded.Amount >= Amount.Amount
            ? PaymentStatus.Refunded
            : PaymentStatus.PartiallyRefunded;

        RaiseDomainEvent(new PaymentRefundedDomainEvent(Id, OrderId, refundAmount, reason));

        return refundResult;
    }

    public Money GetTotalRefundedAmount()
    {
        if (!_refunds.Any())
        {
            return Money.Zero(Amount.Currency);
        }

        decimal totalRefunded = _refunds.Sum(r => r.Amount.Amount);
        return new Money(totalRefunded, Amount.Currency);
    }

    public Money GetRemainingAmount()
    {
        Money totalRefunded = GetTotalRefundedAmount();
        return new Money(Amount.Amount - totalRefunded.Amount, Amount.Currency);
    }

    public bool CanBeRefunded()
    {
        return PaymentStatus == PaymentStatus.Paid || PaymentStatus == PaymentStatus.PartiallyRefunded;
    }


    public Result FailPayment(string reason)
    {
        if (PaymentStatus != PaymentStatus.Pending)
        {
            return Result.Failure(PaymentErrors.PaymentAlreadyProcessed);
        }

        PaymentStatus = PaymentStatus.Failed;

        // TODO2 reason not good
        RaiseDomainEvent(new PaymentFailedDomainEvent(Id, OrderId, reason));

        return Result.Success();
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

    public Result MarkAsProcessing()
    {
        if (PaymentStatus != PaymentStatus.Pending)
        {
            return Result.Failure(PaymentErrors.InvalidStatusTransition);
        }

        PaymentStatus = PaymentStatus.Processing;
        RaiseDomainEvent(new PaymentProcessingStartedDomainEvent(Id, OrderId));

        return Result.Success();
    }

    public Result CancelPayment(string reason)
    {
        if (PaymentStatus is not (PaymentStatus.Pending or PaymentStatus.Processing))
        {
            return Result.Failure(PaymentErrors.CannotCancelProcessedPayment);
        }

        PaymentStatus = PaymentStatus.Cancelled;
        RaiseDomainEvent(new PaymentCancelledDomainEvent(Id, OrderId, reason));

        return Result.Success();
    }

    public Result MarkAsDisputed(string disputeReason)
    {
        if (PaymentStatus != PaymentStatus.Paid)
        {
            return Result.Failure(PaymentErrors.CannotDisputeUnpaidPayment);
        }

        PaymentStatus = PaymentStatus.Disputed;
        RaiseDomainEvent(new PaymentDisputedDomainEvent(Id, OrderId, disputeReason));

        return Result.Success();
    }

    public Result MarkAsExpired()
    {
        if (PaymentStatus is not (PaymentStatus.Pending or PaymentStatus.Processing))
        {
            return Result.Failure(PaymentErrors.CannotExpireProcessedPayment);
        }

        PaymentStatus = PaymentStatus.Expired;
        RaiseDomainEvent(new PaymentExpiredDomainEvent(Id, OrderId));

        return Result.Success();
    }

    public bool IsPartiallyPaid()
    {
        return PaymentStatus == PaymentStatus.PartiallyPaid;
    }

    public bool IsPartiallyRefunded()
    {
        return PaymentStatus == PaymentStatus.PartiallyRefunded;
    }

    public bool IsDisputed()
    {
        return PaymentStatus == PaymentStatus.Disputed;
    }

    public bool IsExpired()
    {
        return PaymentStatus == PaymentStatus.Expired;
    }

    public bool IsCancelled()
    {
        return PaymentStatus == PaymentStatus.Cancelled;
    }

    // Important!
    // Ideally, if our system supports many currencies,
    // we need to create parameter Currency currency
    // this is necessary, for propper payment creation!
    // MANY PAYMENTS
    // public static Result<Payment> Create(Guid orderId)
    // {
    //     var payment = new Payment(
    //         Guid.NewGuid(),
    //         orderId,
    //         Money.Zero(Currency.Eur),
    //         PaymentStatus.Pending,
    //         Money.Zero(Currency.Eur),
    //         Money.Zero(Currency.Eur));
    //
    //     return Result.Success(payment);
    // }

    // MANY PAYMENTS
    // public Result UpdateTotalAmount(Money newTotalAmount)
    // {
    //     if (PaymentStatus == PaymentStatus.Paid)
    //     {
    //         return Result.Failure(PaymentErrors.CannotUpdatePaidPaymentAmount);
    //     }
    //
    //     TotalAmount = newTotalAmount;
    //
    //     Result<Money> outstandingAmountResult = Money.Create(TotalAmount.Amount - PaidAmount.Amount, Currency.Eur);
    //     if (outstandingAmountResult.IsFailure)
    //     {
    //         return Result.Failure(outstandingAmountResult.Error);
    //     }
    //
    //     OutstandingAmount = outstandingAmountResult.Value;
    //
    //     return Result.Success();
    // }

    // Important!
    // Ideally, if our system supports many currencies,
    // we need to create parameter Currency currency
    // this is necessary, for propper payment processing!
    // MANY PAYMENTS
    // public Result ProcessPayment(Money paymentAmount)
    // {
    //     if (PaymentStatus == PaymentStatus.Paid)
    //     {
    //         return Result.Failure(PaymentErrors.PaymentAlreadyCompleted);
    //     }
    //
    //     if (paymentAmount <= Money.Zero(Currency.Eur))
    //     {
    //         return Result.Failure(PaymentErrors.InvalidPaymentAmount);
    //     }
    //
    //     if (paymentAmount > TotalAmount)
    //     {
    //         return Result.Failure(PaymentErrors.PaymentAmountExceedsPaymentTotalAmount);
    //     }
    //
    //     if (paymentAmount > OutstandingAmount)
    //     {
    //         return Result.Failure(PaymentErrors.PaymentAmountExceedsOutstandingAmount);
    //     }
    //
    //     PaidAmount += paymentAmount;
    //
    //     OutstandingAmount = TotalAmount - PaidAmount;
    //
    //     if (PaidAmount == TotalAmount)
    //     {
    //         PaymentStatus = PaymentStatus.Paid;
    //         PaymentCompletedAt = DateTime.UtcNow;
    //         OutstandingAmount = Money.Zero(Currency.Eur);
    //
    //         RaiseDomainEvent(new PaymentCompletedDomainEvent(Id, OrderId, PaidAmount));
    //     }
    //     else if (PaidAmount > Money.Zero(Currency.Eur))
    //     {
    //         PaymentStatus = PaymentStatus.PartiallyPaid;
    //         RaiseDomainEvent(new PartialPaymentReceivedDomainEvent(Id, OrderId, PaidAmount, OutstandingAmount));
    //     }
    //
    //     return Result.Success();
    // }

    // Important!
    // Ideally, if our system supports many currencies,
    // we need to create parameter Currency currency
    // this is necessary, for propper payment refund!
    // MANY PAYMENTS
    // public Result RefundPayment(Money refundAmount, RefundReason reason)
    // {
    //     if (PaymentStatus != PaymentStatus.Paid)
    //     {
    //         return Result.Failure(PaymentErrors.CannotRefundUnpaidPayment);
    //     }
    //
    //     if (refundAmount > PaidAmount)
    //     {
    //         return Result.Failure(PaymentErrors.RefundAmountExceedsPaidAmount);
    //     }
    //
    //     if (refundAmount <= Money.Zero(Currency.Eur))
    //     {
    //         return Result.Failure(PaymentErrors.InvalidRefundAmount);
    //     }
    //
    //     PaidAmount -= refundAmount;
    //     OutstandingAmount = TotalAmount - PaidAmount;
    //
    //     PaymentStatus = PaidAmount.IsZero() ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
    //
    //     RaiseDomainEvent(new OrderRefundProcessedDomainEvent(Id, OrderId, refundAmount, reason));
    //
    //     return Result.Success();
    // }

    public bool IsCompleted()
    {
        return PaymentStatus == PaymentStatus.Paid;
    }

    // MANY PAYMENTS
    public bool IsFailed()
    {
        return PaymentStatus == PaymentStatus.Failed;
    }

    public bool IsRefunded()
    {
        return PaymentStatus == PaymentStatus.Refunded;
    }


    // Important!
    // Ideally, if our system supports many currencies,
    // we need to create parameter Currency currency
    // this is necessary, for outstanding amount detection!
    // MANY PAYMENTS
    // public bool HasOutstandingAmount()
    // {
    //     return OutstandingAmount > Money.Zero(Currency.Eur);
    // }
    //
    // public bool CanProcessPayment()
    // {
    //     return PaymentStatus != PaymentStatus.Paid;
    // }
}
