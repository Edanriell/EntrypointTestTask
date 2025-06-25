using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Shared;

namespace Server.Domain.Payments;

public sealed class OrderPaymentService
{
    public Result ProcessPaymentWithAutomaticOrderConfirmation(Payment payment, Order order, Money paymentAmount)
    {
        if (payment.OrderId != order.Id)
        {
            return Result.Failure(PaymentErrors.PaymentOrderMismatch);
        }

        Result paymentResult = payment.ProcessPayment(paymentAmount);
        if (paymentResult.IsFailure)
        {
            return paymentResult;
        }

        if (payment.PaymentStatus == PaymentStatus.Paid)
        {
            Result confirmResult = order.ConfirmWithoutPaymentCheck();
            if (confirmResult.IsFailure)
            {
                return confirmResult;
            }
        }

        return Result.Success();
    }

    public Result RefundPaymentWithOrderUpdate(Payment payment, Order order, Money refundAmount, RefundReason reason)
    {
        if (payment.OrderId != order.Id)
        {
            return Result.Failure(PaymentErrors.PaymentOrderMismatch);
        }

        Result refundResult = payment.RefundPayment(refundAmount, reason);
        if (refundResult.IsFailure)
        {
            return refundResult;
        }

        if (payment.PaymentStatus == PaymentStatus.Refunded)
        {
            Result returnResult = order.MarkAsReturnedDueToRefund(reason);
            if (returnResult.IsFailure)
            {
                return returnResult;
            }
        }

        return Result.Success();
    }

    public Result UpdatePaymentTotalFromOrder(Payment payment, Order order)
    {
        if (payment.OrderId != order.Id)
        {
            return Result.Failure(PaymentErrors.PaymentOrderMismatch);
        }

        Money orderTotal = order.CalculateTotal();

        return payment.UpdateTotalAmount(orderTotal);
    }

    public bool CanConfirmOrder(Payment payment, Order order)
    {
        return payment.OrderId == order.Id &&
            payment.PaymentStatus == PaymentStatus.Paid &&
            order.Status == OrderStatus.Pending;
    }
}
