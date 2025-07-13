using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Shared;

namespace Server.Domain.Payments;

public sealed class OrderPaymentService
{
    public Result CanConfirmOrderWithPayments(Order order)
    {
        if (order.Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        if (!order.Payments.Any())
        {
            return Result.Failure(PaymentErrors.NoPaymentsFound);
        }

        // Check if order is fully paid
        if (!order.IsFullyPaid())
        {
            return Result.Failure(OrderErrors.InsufficientPayment);
        }

        // Check if there are any failed or disputed payments
        if (order.Payments.Any(p => p.PaymentStatus is PaymentStatus.Failed or PaymentStatus.Disputed))
        {
            return Result.Failure(PaymentErrors.CannotConfirmOrderWithFailedPayments);
        }

        return Result.Success();
    }

    public Result ProcessFullRefundForOrder(Order order, RefundReason reason)
    {
        if (order.Status is OrderStatus.Cancelled or OrderStatus.Returned)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        var paidPayments = order.Payments
            .Where(p => p.PaymentStatus is PaymentStatus.Paid or PaymentStatus.PartiallyRefunded)
            .ToList();

        if (!paidPayments.Any())
        {
            return Result.Failure(PaymentErrors.NoPaymentsToRefund);
        }

        // Process refunds for all paid payments
        foreach (Payment payment in paidPayments)
        {
            Money remainingAmount = payment.GetRemainingAmount();
            if (remainingAmount.Amount > 0)
            {
                Result<Refund> refundResult = payment.ProcessRefund(remainingAmount, reason);
                if (refundResult.IsFailure)
                {
                    return Result.Failure(refundResult.Error);
                }
            }
        }

        // Mark order as returned if all payments are fully refunded
        bool allPaymentsRefunded = order.Payments.All(p =>
            p.PaymentStatus is PaymentStatus.Refunded or PaymentStatus.Failed or PaymentStatus.Cancelled);

        if (allPaymentsRefunded)
        {
            return order.MarkAsReturnedDueToRefund(reason);
        }

        return Result.Success();
    }

    public Money GetTotalPaidAmount(Order order)
    {
        return order.GetTotalPaidAmount();
    }

    public Money GetTotalOutstandingAmount(Order order)
    {
        return order.GetTotalOutstandingAmount();
    }

    public bool HasPendingPayments(Order order)
    {
        return order.Payments.Any(p => p.PaymentStatus is PaymentStatus.Pending or PaymentStatus.Processing);
    }

    public bool HasFailedPayments(Order order)
    {
        return order.Payments.Any(p => p.PaymentStatus == PaymentStatus.Failed);
    }

    public bool HasDisputedPayments(Order order)
    {
        return order.Payments.Any(p => p.PaymentStatus == PaymentStatus.Disputed);
    }
}
