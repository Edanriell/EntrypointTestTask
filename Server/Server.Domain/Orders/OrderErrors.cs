using Server.Domain.Abstractions;

namespace Server.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error NotFound = new(
        "Order.NotFound",
        "Order not found");

    public static readonly Error EmptyOrder = new(
        "Order.EmptyOrder",
        "Order must contain at least one product");

    public static readonly Error CannotModifyNonPendingOrder = new(
        "Order.CannotModifyNonPending",
        "Can only modify pending orders");

    public static readonly Error InvalidQuantity = new(
        "Order.InvalidQuantity",
        "Quantity must be greater than zero");

    public static readonly Error CurrencyMismatch = new(
        "Order.CurrencyMismatch",
        "All prices must use the same currency");

    public static readonly Error ProductNotFound = new(
        "Order.ProductNotFound",
        "Product not found in order");

    public static readonly Error CannotRemoveLastProduct = new(
        "Order.CannotRemoveLastProduct",
        "Cannot remove the last product from order");

    public static readonly Error InvalidStatusTransition = new(
        "Order.InvalidStatusTransition",
        "Invalid order status transition");

    public static readonly Error CannotCancelShippedOrder = new(
        "Order.CannotCancelShipped",
        "Cannot cancel shipped orders");

    public static readonly Error InvalidTrackingNumber = new(
        "Order.InvalidTrackingNumber",
        "Invalid tracking number");

    public static readonly Error CanOnlyReturnDeliveredOrders = new(
        "Order.CanOnlyReturnDelivered",
        "Can only return delivered orders");

    public static readonly Error ReturnWindowExpired = new(
        "Order.ReturnWindowExpired",
        "Return window has expired");

    public static readonly Error CannotChangeShippingAddressAfterProcessing = new(
        "Order.CannotChangeShippingAddress",
        "Cannot change shipping address after processing started");

    public static readonly Error OrderAlreadyPaid = new(
        "Order.AlreadyPaid",
        "Order is already paid");

    public static readonly Error PaymentCurrencyMismatch = new(
        "Order.PaymentCurrencyMismatch",
        "Payment currency must match order currency");

    public static readonly Error CannotRefundUnpaidOrder = new(
        "Order.CannotRefundUnpaid",
        "Cannot refund unpaid order");

    public static readonly Error RefundAmountExceedsPaidAmount = new(
        "Order.RefundAmountExceeds",
        "Refund amount exceeds paid amount");

    public static readonly Error CannotConfirmUnpaidOrder = new(
        "Order.CannotConfirmUnpaid",
        "Cannot confirm unpaid order");

    public static readonly Error InsufficientPayment = new(
        "Order.InsufficientPayment",
        "Payment amount is insufficient");

    public static readonly Error CannotReturnOrderInCurrentStatus = new(
        "Order.CannotReturnOrderInCurrentStatus",
        "Order cannot be returned in its current status");

    public static readonly Error CannotProcessPaymentForNonPendingOrder = new(
        "Order.CannotProcessPaymentForNonPendingOrder",
        "Cannot process payment for non-pending order");
}
