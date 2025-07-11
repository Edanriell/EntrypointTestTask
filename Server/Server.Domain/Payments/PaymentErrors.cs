using Server.Domain.Abstractions;

namespace Server.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error PaymentOrderMismatch = new(
        "Payment.PaymentOrderMismatch",
        "Payment does not belong to the specified order"
    );

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
}
