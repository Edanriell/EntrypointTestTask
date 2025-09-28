using Server.Domain.Abstractions;

namespace Server.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error CurrencyMismatch = new(
        "Order.CurrencyMismatch",
        "All prices must use the same currency");

    public static readonly Error PendingPaymentExists = new(
        "Payment.PendingPaymentExists",
        "Order already has a pending payment. Please wait for it to complete before adding another payment.");

    public static Error InvalidPaymentAmount => new(
        "Payment.InvalidAmount",
        "Payment amount must be greater than zero");

    public static Error CannotRefundUnpaidPayment => new(
        "Payment.CannotRefundUnpaid",
        "Cannot refund a payment that hasn't been completed");

    public static Error PaymentAlreadyProcessed => new(
        "Payment.AlreadyProcessed",
        "Payment has already been processed");

    public static Error InvalidStatusTransition => new(
        "Payment.InvalidStatusTransition",
        "Invalid payment status transition");

    public static Error CannotCancelProcessedPayment => new(
        "Payment.CannotCancelProcessedPayment",
        "Cannot cancel a payment that has already been processed");

    public static Error NotFound => new(
        "Payment.NotFound",
        "Payment not found");

    public static Error NoPaymentsFound => new(
        "Payment.NoPaymentsFound",
        "No payments found for the order");

    public static Error NoPaymentsToRefund => new(
        "Payment.NoPaymentsToRefund",
        "No payments available for refund");

    public static Error InvalidPaymentMethod => new(
        "Payment.InvalidPaymentMethod",
        "Invalid payment method provided");

    public static Error PaymentFailed => new(
        "Payment.Failed",
        "Payment failed");

    public static Error PaymentExpired => new(
        "Payment.Expired",
        "Payment expired");

    public static Error MixedCurrenciesNotAllowed => new(
        "Payment.MixedCurrenciesNotAllowed",
        "Payment currency must match order currency");
}
