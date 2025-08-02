using Server.Domain.Abstractions;

namespace Server.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error EmptyCurrencyCode = new(
        "Currency.EmptyCode",
        "Currency code cannot be empty");

    public static readonly Error InvalidCurrencyCode = new(
        "Currency.InvalidCode",
        "The specified currency code is not supported");

    public static readonly Error CurrencyMismatch = new(
        "Order.CurrencyMismatch",
        "All prices must use the same currency");

    public static readonly Error PendingPaymentExists = new(
        "Payment.PendingPaymentExists",
        "Order already has a pending payment. Please wait for it to complete before adding another payment.");

    public static readonly Error PendingPaymentsExceedRemainingAmount = new(
        "Payment.PendingPaymentsExceedRemainingAmount",
        "Total pending payments and new payment amount exceed the remaining order amount.");

    public static Error PaymentAlreadyCompleted => new(
        "Payment.AlreadyCompleted",
        "Payment has already been completed");

    public static Error InvalidPaymentAmount => new(
        "Payment.InvalidAmount",
        "Payment amount must be greater than zero");

    public static Error PaymentAmountExceedsPaymentTotalAmount => new(
        "Payment.PaymentAmountExceedsPaymentTotalAmount",
        "Payment amount cannot exceed the total amount of the payment");

    public static Error CannotRefundUnpaidPayment => new(
        "Payment.CannotRefundUnpaid",
        "Cannot refund a payment that hasn't been completed");

    public static Error RefundAmountExceedsPaidAmount => new(
        "Payment.RefundExceedsPaid",
        "Refund amount cannot exceed the paid amount");

    public static Error InvalidRefundAmount => new(
        "Payment.InvalidRefundAmount",
        "Refund amount must be greater than zero");

    public static Error CannotUpdatePaidPaymentAmount => new(
        "Payment.CannotUpdatePaidAmount",
        "Cannot update total amount of a completed payment");

    public static Error PaymentNotFound => new(
        "Payment.PaymentNotFound",
        "Payment not found");

    public static Error PaymentAmountExceedsOutstandingAmount => new(
        "Payment.PaymentAmountExceedsOutstandingAmount",
        "Payment amount cannot exceed the outstanding amount");

    public static Error InvalidRefundReason => new(
        "Payment.InvalidRefundReason",
        "Invalid refund reason provided");

    public static Error PaymentAlreadyProcessed => new(
        "Payment.AlreadyProcessed",
        "Payment has already been processed");

    public static Error RefundAlreadyProcessed => new(
        "Payment.RefundAlreadyProcessed",
        "Refund has already been processed");

    public static Error InvalidStatusTransition => new(
        "Payment.InvalidStatusTransition",
        "Invalid payment status transition");

    public static Error CannotCancelProcessedPayment => new(
        "Payment.CannotCancelProcessedPayment",
        "Cannot cancel a payment that has already been processed");

    public static Error CannotDisputeUnpaidPayment => new(
        "Payment.CannotDisputeUnpaidPayment",
        "Cannot dispute a payment that hasn't been paid");

    public static Error CannotExpireProcessedPayment => new(
        "Payment.CannotExpireProcessedPayment",
        "Cannot expire a payment that has already been processed");

    public static Error NotFound => new(
        "Payment.NotFound",
        "Payment not found");

    public static Error NoPaymentsFound => new(
        "Payment.NoPaymentsFound",
        "No payments found for the order");

    public static Error CannotConfirmOrderWithFailedPayments => new(
        "Payment.CannotConfirmOrderWithFailedPayments",
        "Cannot confirm order with failed or disputed payments");

    public static Error NoPaymentsToRefund => new(
        "Payment.NoPaymentsToRefund",
        "No payments available for refund");

    public static Error PaymentOrderMismatch => new(
        "Payment.PaymentOrderMismatch",
        "Payment does not belong to the specified order");

    public static Error InvalidPaymentMethod => new(
        "Payment.InvalidPaymentMethod",
        "Invalid payment method provided");

    public static Error PaymentExpired => new(
        "Payment.Expired",
        "Cannot process expired payment");

    public static Error PaymentFailed => new(
        "Payment.Failed",
        "Cannot process failed payment");
}
