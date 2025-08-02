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

    // public static readonly Error InvalidStatusTransition = new(
    //     "Order.InvalidStatusTransition",
    //     "Invalid order status transition");

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

    public static readonly Error CannotChangeOrderInfoAfterProcessing = new(
        "Order.CannotChangeOrderInfoAfterProcessing",
        "Cannot change order info after processing started");

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

    // public static readonly Error CannotAssignPaymentToInactiveOrder = new(
    //     "Order.CannotAssignPaymentToInactiveOrder",
    //     "Cannot assign payment to inactive order");
    //
    // public static readonly Error PaymentAlreadyAssigned = new(
    //     "Order.PaymentAlreadyAssigned", "Payment already assigned");

    public static readonly Error OrderProductsCurrencyMismatch = new(
        "Order.OrderProductsCurrencyMismatch",
        "All order products must have the same currency");

    public static readonly Error CannotAddPaymentToClosedOrder = new(
        "Order.CannotAddPaymentToClosedOrder",
        "Cannot add payment to cancelled or completed order");

    public static readonly Error MixedCurrenciesNotAllowed = new(
        "Order.MixedCurrenciesNotAllowed",
        "Cannot add products with different currencies to the same order");

    public static readonly Error CannotAddPaymentInCurrentStatus = new(
        "Orders.CannotAddPaymentInCurrentStatus",
        "Cannot add payment to order in current status. Payments are only allowed for pending orders.");

    public static readonly Error OrderMustBeFullyPaidBeforeConfirmation = new(
        "Orders.OrderMustBeFullyPaidBeforeConfirmation",
        "Order must be fully paid before it can be confirmed.");

    public static readonly Error CannotConfirmOrderWithPendingPayments = new(
        "Orders.CannotConfirmOrderWithPendingPayments",
        "Cannot confirm order while there are pending payments. All payments must be processed.");

    public static readonly Error PaymentAmountExceedsRemainingAmount = new(
        "Orders.PaymentAmountExceedsRemainingAmount",
        "Payment amount exceeds the remaining amount to be paid.");

    public static readonly Error PendingPaymentsExceedRemainingAmount = new(
        "Orders.PendingPaymentsExceedRemainingAmount",
        "Total pending payments exceed the remaining amount to be paid.");

    public static readonly Error PaymentNotFound = new(
        "Order.PaymentNotFound",
        "The payment with the specified identifier was not found");

    public static readonly Error ProductAlreadyExists = new(
        "Order.ProductAlreadyExists",
        "The product already exists in this order");

    public static readonly Error NoProductsToAdd = new(
        "Order.NoProductsToAdd",
        "No products were provided to add to the order");

    public static readonly Error CanOnlyRefundReturnedOrCancelledOrders = new(
        "Order.CanOnlyRefundReturnedOrCancelledOrders",
        "Refunds can only be processed for returned or cancelled orders");

    public static readonly Error NoPaymentsToRefund = new(
        "Order.NoPaymentsToRefund",
        "No payments found to refund");

    public static readonly Error InvalidQuantityToRemove = new(
        "Order.InvalidQuantityToRemove",
        "Quantity to remove must be greater than zero");

    public static readonly Error CannotRemoveMoreThanAvailable = new(
        "Order.CannotRemoveMoreThanAvailable",
        "Cannot remove more quantity than what is available in the order");

    public static Error CannotAddPaymentToInactiveOrder => new(
        "Order.CannotAddPaymentToInactiveOrder",
        "Cannot add payment to cancelled or returned order");

    public static Error CannotAssignPaymentToInactiveOrder => new(
        "Order.CannotAssignPaymentToInactiveOrder",
        "Cannot assign payment to inactive order");

    public static Error PaymentAlreadyAssigned => new(
        "Order.PaymentAlreadyAssigned",
        "Payment is already assigned to this order");

    public static Error CannotCompleteOrderWithinReturnWindow => new(
        "Order.CannotCompleteOrderWithinReturnWindow",
        "Cannot complete order within return window period");

    public static Error InvalidStatusTransition => new(
        "Order.InvalidStatusTransition",
        "Invalid order status transition");

    public static Error CannotRemoveMoreThanOrderedQuantity => new(
        "Order.CannotRemoveMoreThanOrderedQuantity",
        "Cannot remove more quantity than what is currently in the order");

    public static Error CannotAddPaymentToCancelledOrder => new(
        "Order.CannotAddPaymentToCancelledOrder",
        "Cannot add payment to cancelled order");

    public static Error CannotCancelAlreadyCancelledOrder => new(
        "Order.CannotCancelAlreadyCancelledOrder",
        "Cannot cancel already cancelled order");
}
