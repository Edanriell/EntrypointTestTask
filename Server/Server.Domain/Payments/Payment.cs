using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Orders.Events;
using Server.Domain.Payments.Events;
using Server.Domain.Shared;

namespace Server.Domain.Payments;

public sealed class Payment : Entity
{
    private Payment(
        Guid id, Guid orderId, Money totalAmount, PaymentStatus paymentStatus, Money paidAmount,
        Money outstandingAmount) : base(id)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
        PaymentStatus = paymentStatus;
        PaidAmount = paidAmount;
        OutstandingAmount = outstandingAmount;
    }

    // We must add that of type constructors for EF
    private Payment() { }

    public Guid OrderId { get; }
    public Money TotalAmount { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public Money PaidAmount { get; private set; }
    public Money OutstandingAmount { get; private set; }
    public DateTime? PaymentCompletedAt { get; private set; }

    // Navigation property
    public Order Order { get; private set; }

    public static Result<Payment> Create(Guid orderId)
    {
        var payment = new Payment(
            Guid.NewGuid(),
            orderId,
            Money.Zero(),
            PaymentStatus.Pending,
            Money.Zero(),
            Money.Zero());

        return Result.Success(payment);
    }

    public Result UpdateTotalAmount(Money newTotalAmount)
    {
        if (PaymentStatus == PaymentStatus.Paid)
        {
            return Result.Failure(PaymentErrors.CannotUpdatePaidPaymentAmount);
        }

        TotalAmount = newTotalAmount;
        OutstandingAmount = TotalAmount - PaidAmount;

        return Result.Success();
    }

    public Result ProcessPayment(Money paymentAmount)
    {
        if (PaymentStatus == PaymentStatus.Paid)
        {
            return Result.Failure(PaymentErrors.PaymentAlreadyCompleted);
        }

        if (paymentAmount <= Money.Zero())
        {
            return Result.Failure(PaymentErrors.InvalidPaymentAmount);
        }

        if (paymentAmount > TotalAmount)
        {
            return Result.Failure(PaymentErrors.PaymentAmountExceedsPaymentTotalAmount);
        }

        if (paymentAmount > OutstandingAmount)
        {
            return Result.Failure(PaymentErrors.PaymentAmountExceedsOutstandingAmount);
        }

        PaidAmount += paymentAmount;
        OutstandingAmount = TotalAmount - PaidAmount;

        if (PaidAmount == TotalAmount)
        {
            PaymentStatus = PaymentStatus.Paid;
            PaymentCompletedAt = DateTime.UtcNow;
            OutstandingAmount = Money.Zero();

            RaiseDomainEvent(new PaymentCompletedDomainEvent(Id, OrderId, PaidAmount));
        }
        else if (PaidAmount > Money.Zero())
        {
            PaymentStatus = PaymentStatus.PartiallyPaid;
            RaiseDomainEvent(new PartialPaymentReceivedDomainEvent(Id, OrderId, PaidAmount, OutstandingAmount));
        }

        return Result.Success();
    }

    public Result RefundPayment(Money refundAmount, RefundReason reason)
    {
        if (PaymentStatus != PaymentStatus.Paid)
        {
            return Result.Failure(PaymentErrors.CannotRefundUnpaidPayment);
        }

        if (refundAmount > PaidAmount)
        {
            return Result.Failure(PaymentErrors.RefundAmountExceedsPaidAmount);
        }

        if (refundAmount <= Money.Zero())
        {
            return Result.Failure(PaymentErrors.InvalidRefundAmount);
        }

        PaidAmount -= refundAmount;
        OutstandingAmount = TotalAmount - PaidAmount;

        PaymentStatus = PaidAmount.IsZero() ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;

        RaiseDomainEvent(new OrderRefundProcessedDomainEvent(Id, OrderId, refundAmount, reason));

        return Result.Success();
    }

    public bool IsCompleted()
    {
        return PaymentStatus == PaymentStatus.Paid;
    }

    public bool HasOutstandingAmount()
    {
        return OutstandingAmount > Money.Zero();
    }

    public bool CanProcessPayment()
    {
        return PaymentStatus != PaymentStatus.Paid;
    }
}
