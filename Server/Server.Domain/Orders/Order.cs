using Server.Domain.Abstractions;
using Server.Domain.OrderProducts;
using Server.Domain.Orders.Events;
using Server.Domain.Payments;
using Server.Domain.Refunds;
using Server.Domain.Shared;

namespace Server.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderProduct> _orderProducts = new();
    private readonly List<Guid> _paymentIds = new();

    private Order(
        Guid id,
        Guid clientId,
        OrderNumber orderNumber,
        Currency currency,
        OrderInfo? info,
        Money totalAmount,
        Money paidAmount,
        Money totalRefundedAmount,
        Address shippingAddress,
        DateTime createdAt,
        OrderStatus orderStatus
    ) : base(id)
    {
        ClientId = clientId;
        OrderNumber = orderNumber;
        Currency = currency;
        Info = info;
        TotalAmount = totalAmount;
        PaidAmount = paidAmount;
        TotalRefundedAmount = totalRefundedAmount;
        ShippingAddress = shippingAddress;
        CreatedAt = createdAt;
        Status = orderStatus;
    }

    private Order() { }

    public Guid ClientId { get; }
    public OrderNumber OrderNumber { get; private set; }
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public CancellationReason? CancellationReason { get; private set; }
    public ReturnReason? ReturnReason { get; private set; }
    public RefundReason? RefundReason { get; private set; }
    public OrderInfo? Info { get; private set; }
    public TrackingNumber TrackingNumber { get; private set; }
    public Courier? Courier { get; private set; }
    public DateTime? EstimatedDeliveryDate { get; private set; }

    // Order currency is set once, on the first product adding.
    public Currency Currency { get; }
    public Money TotalAmount { get; private set; }
    public Money PaidAmount { get; private set; }
    public Money RemainingAmount => TotalAmount - PaidAmount;
    public IReadOnlyCollection<OrderProduct> OrderProducts => _orderProducts.AsReadOnly();
    public IReadOnlyCollection<Guid> PaymentIds => _paymentIds.AsReadOnly();
    public bool HasPendingPayments { get; private set; }

    public bool HasActivePayments { get; private set; }

    public bool HasFailedPayments { get; private set; }

    public bool HasDisputedPayments { get; private set; }

    public bool CanBeShipped => Status == OrderStatus.Confirmed;
    public Money TotalRefundedAmount { get; private set; } = Money.Zero();
    public bool HasRefunds => TotalRefundedAmount.Amount > 0;

    public bool CanBeModified()
    {
        return Status == OrderStatus.Pending;
    }


    public static Result<Order> Create(
        Guid clientId,
        OrderNumber orderNumber,
        Currency orderCurrency,
        OrderInfo? info,
        Address shippingAddress)
    {
        var order = new Order(
            Guid.NewGuid(),
            clientId,
            orderNumber,
            orderCurrency,
            info,
            Money.Zero(orderCurrency),
            Money.Zero(orderCurrency),
            Money.Zero(orderCurrency),
            shippingAddress,
            DateTime.UtcNow,
            OrderStatus.Pending
        );

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, order.ClientId));

        return Result.Success(order);
    }

    public Result AddProduct(OrderProduct orderProduct)
    {
        if (Status is not OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        if (Currency != orderProduct.UnitPrice.Currency)
        {
            return Result.Failure(OrderErrors.MixedCurrenciesNotAllowed);
        }

        if (HasProduct(orderProduct.ProductId))
        {
            return UpdateExistingProductQuantity(orderProduct.ProductId, orderProduct.Quantity);
        }

        return AddNewProduct(orderProduct);
    }

    public Result UpdateExistingProductQuantity(Guid productId, Quantity additionalQuantity)
    {
        if (Status is not OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        OrderProduct? existingProduct = _orderProducts.FirstOrDefault(op => op.ProductId == productId);
        if (existingProduct is null)
        {
            return Result.Failure(OrderErrors.ProductNotFound);
        }

        Result updateResult = existingProduct.UpdateQuantity(existingProduct.Quantity + additionalQuantity);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        RecalculateTotalAmount();
        return Result.Success();
    }

    public Result AddNewProduct(OrderProduct orderProduct)
    {
        if (Status is not OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        if (Currency != orderProduct.UnitPrice.Currency)
        {
            return Result.Failure(OrderErrors.MixedCurrenciesNotAllowed);
        }

        // ✅ Check if product already exists (safety check)
        if (_orderProducts.Any(op => op.ProductId == orderProduct.ProductId))
        {
            return Result.Failure(OrderErrors.ProductAlreadyExists);
        }

        _orderProducts.Add(orderProduct);
        RecalculateTotalAmount();

        return Result.Success();
    }

    public Result RemoveProduct(Guid productId)
    {
        if (Status is not OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        OrderProduct? orderProduct = _orderProducts?.FirstOrDefault(op => op.ProductId == productId);
        if (orderProduct is null)
        {
            return Result.Failure(OrderErrors.ProductNotFound);
        }

        _orderProducts?.Remove(orderProduct);

        if (_orderProducts is not { Count: > 0 })
        {
            return Result.Failure(OrderErrors.CannotRemoveLastProduct);
        }

        RecalculateTotalAmount();

        return Result.Success();
    }

    public Result UpdateShippingAddress(Address newAddress)
    {
        if (Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
        {
            return Result.Failure(OrderErrors.CannotChangeShippingAddressAfterProcessing);
        }

        Address oldAddress = ShippingAddress;
        ShippingAddress = newAddress;

        RaiseDomainEvent(new OrderShippingAddressChangedDomainEvent(Id, oldAddress, newAddress));

        return Result.Success();
    }

    public Result UpdateInfo(OrderInfo newInfo)
    {
        if (Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
        {
            return Result.Failure(OrderErrors.CannotChangeOrderInfoAfterProcessing);
        }

        Info = newInfo;

        return Result.Success();
    }

    // Example of synchronous validation within Order aggregate root. 
    public Result ValidatePayment(Money amount)
    {
        if (!CanAcceptPayments())
        {
            return Result.Failure(OrderErrors.CannotAddPaymentInCurrentStatus);
        }

        if (amount > RemainingAmount)
        {
            return Result.Failure(OrderErrors.PaymentAmountExceedsRemainingAmount);
        }

        return Result.Success();
    }

    // Important !!!
    // Important !!!
    // Important !!!
    // Important !!!
    // Important !!!
    // Important !!!
    // Important !!!
    // Rename to AttachPaymentToOrder
    public Result RecordOrderPayment(Guid paymentId, Money amount)
    {
        if (Status == OrderStatus.Cancelled)
        {
            return Result.Failure(OrderErrors.CannotAddPaymentToCancelledOrder);
        }

        _paymentIds.Add(paymentId);

        RaiseDomainEvent(new PaymentAddedDomainEvent(Id, paymentId, amount));

        return Result.Success();
    }

    public void UpdateOrderTotalPaidAmount(Money amount)
    {
        Money newPaidAmount = PaidAmount + amount;
        UpdatePaidAmount(newPaidAmount);
    }

    public bool IsFullyPaid()
    {
        return PaidAmount.Amount == TotalAmount.Amount;
    }

    public bool HasSufficientPaymentCoverage(Money pendingAmount)
    {
        decimal totalCoverage = PaidAmount.Amount + pendingAmount.Amount;
        return totalCoverage >= TotalAmount.Amount;
    }

    public bool CanBeFullyPaid(Money additionalAmount)
    {
        decimal totalWithAdditional = PaidAmount.Amount + additionalAmount.Amount;
        return totalWithAdditional >= TotalAmount.Amount;
    }

    public bool HasPartialPayment()
    {
        return PaidAmount.Amount > 0 && !IsFullyPaid();
    }

    public Result UpdateProductQuantity(Guid productId, Quantity newQuantity)
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        OrderProduct? orderProduct = _orderProducts?.FirstOrDefault(op => op.ProductId == productId);
        if (orderProduct is null)
        {
            return Result.Failure(OrderErrors.ProductNotFound);
        }

        Result result = orderProduct.UpdateQuantity(newQuantity);
        if (result.IsFailure)
        {
            return result;
        }

        RecalculateTotalAmount();

        return Result.Success();
    }

    public Result Confirm()
    {
        if (Status is not OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        if (_orderProducts is not { Count: > 0 })
        {
            return Result.Failure(OrderErrors.EmptyOrder);
        }

        if (_paymentIds is not { Count: > 0 })
        {
            return Result.Failure(PaymentErrors.NoPaymentsFound);
        }

        if (!IsFullyPaid())
        {
            return Result.Failure(OrderErrors.OrderMustBeFullyPaidBeforeConfirmation);
        }

        if (HasPendingPayments)
        {
            return Result.Failure(OrderErrors.CannotConfirmOrderWithPendingPayments);
        }

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        RaiseDomainEvent(new OrderConfirmedDomainEvent(Id, ClientId, CalculateTotalAmount(Currency)));

        return Result.Success();
    }

    public Result StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        Status = OrderStatus.Processing;
        RaiseDomainEvent(new OrderProcessingStartedDomainEvent(Id));

        return Result.Success();
    }

    public Result MarkReadyForShipment()
    {
        if (Status != OrderStatus.Processing)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        Status = OrderStatus.ReadyForShipment;
        RaiseDomainEvent(new OrderReadyForShipmentDomainEvent(Id));

        return Result.Success();
    }

    public Result Ship(TrackingNumber trackingNumber, Courier courier, DateTime estimatedDeliveryDate)
    {
        if (Status != OrderStatus.ReadyForShipment)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        if (string.IsNullOrWhiteSpace(trackingNumber.Value))
        {
            return Result.Failure(OrderErrors.InvalidTrackingNumber);
        }

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        TrackingNumber = trackingNumber;
        Courier = courier;
        EstimatedDeliveryDate = estimatedDeliveryDate;

        RaiseDomainEvent(new OrderShippedDomainEvent(Id));

        return Result.Success();
    }

    public Result MarkOutForDelivery(DateTime? estimatedDeliveryDate)
    {
        if (Status != OrderStatus.Shipped)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        EstimatedDeliveryDate = estimatedDeliveryDate;
        Status = OrderStatus.OutForDelivery;
        RaiseDomainEvent(new OrderOutForDeliveryDomainEvent(Id));

        return Result.Success();
    }

    public Result MarkAsDelivered()
    {
        if (Status != OrderStatus.OutForDelivery)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;

        RaiseDomainEvent(new OrderDeliveredDomainEvent(Id, ClientId));

        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != OrderStatus.Delivered)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        Status = OrderStatus.Completed;

        RaiseDomainEvent(new OrderCompletedDomainEvent(Id, ClientId));

        return Result.Success();
    }

    public Result Cancel(CancellationReason reason)
    {
        if (Status is OrderStatus.Cancelled)
        {
            return Result.Failure(OrderErrors.CannotCancelAlreadyCancelledOrder);
        }

        // TODO22 Can cancel only pending orders !!! 
        if (Status is OrderStatus.Shipped or OrderStatus.OutForDelivery or OrderStatus.Delivered
            or OrderStatus.Completed or OrderStatus.Returned)
        {
            return Result.Failure(OrderErrors.CannotCancelShippedOrder);
        }

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;

        RaiseDomainEvent(new OrderCancelledDomainEvent(Id, Status, reason));

        return Result.Success();
    }

    public Result Return(ReturnReason reason)
    {
        if (Status != OrderStatus.Completed)
        {
            return Result.Failure(OrderErrors.CanOnlyReturnDeliveredOrders);
        }

        int daysSinceDelivery = (DateTime.UtcNow - DeliveredAt!.Value).Days;
        if (daysSinceDelivery > 30)
        {
            return Result.Failure(OrderErrors.ReturnWindowExpired);
        }

        Status = OrderStatus.Returned;
        ReturnReason = reason;

        RaiseDomainEvent(new OrderReturnedDomainEvent(Id, ClientId, reason));

        return Result.Success();
    }

    public Result MarkAsReturnedDueToRefund(RefundReason refundReason)
    {
        // ✅ FIXED: Only allow refunds for Returned or Cancelled orders
        if (Status is not (OrderStatus.Returned or OrderStatus.Cancelled))
        {
            return Result.Failure(OrderErrors.CanOnlyRefundReturnedOrCancelledOrders);
        }

        // ✅ Can't refund if no payments were made
        if (PaidAmount.Amount <= 0)
        {
            return Result.Failure(OrderErrors.NoPaymentsToRefund);
        }

        // ✅ Update refund information
        RefundReason = refundReason;

        // ✅ Raise domain event for refund processing
        // RaiseDomainEvent(new OrderRefundProcessedDomainEvent(Id, ClientId, refundReason));

        return Result.Success();
    }


    public Money CalculateTotalAmount()
    {
        if (_orderProducts == null || !_orderProducts.Any())
        {
            return Money.Zero(Currency);
        }

        Currency currency = Currency;

        return _orderProducts
            .Select(op => op.TotalPrice)
            .Aggregate(Money.Zero(currency), (total, price) => total + price);
    }

    public Money CalculateTotalAmount(Currency currency)
    {
        return _orderProducts?
            .Select(op => op.TotalPrice)
            .Aggregate(Money.Zero(currency), (total, price) => total + price) ?? Money.Zero(currency);
    }

    public void RecalculateTotalAmount()
    {
        Money newTotal = CalculateTotalAmount(Currency);

        TotalAmount = newTotal;

        RaiseDomainEvent(new OrderTotalChangedDomainEvent(Id, newTotal));
    }

    public bool HasProduct(Guid productId)
    {
        return _orderProducts.Any(op => op.ProductId == productId);
    }

    public Result<Quantity> GetProductQuantity(Guid productId)
    {
        OrderProduct? product = _orderProducts.FirstOrDefault(op => op.ProductId == productId);
        if (product == null)
        {
            return Result.Failure<Quantity>(OrderErrors.ProductNotFound);
        }

        return Result.Success(product.Quantity);
    }

    public Result<Money> GetProductTotal(Guid productId)
    {
        OrderProduct? product = _orderProducts?.FirstOrDefault(op => op.ProductId == productId);
        if (product == null)
        {
            return Result.Failure<Money>(OrderErrors.ProductNotFound);
        }

        return Result.Success(product.TotalPrice);
    }

    private bool CanAcceptPayments()
    {
        return Status is OrderStatus.Pending;
    }

    public bool CanBeConfirmed()
    {
        return Status == OrderStatus.Pending
            && _orderProducts?.Count > 0
            && _paymentIds.Count > 0;
    }

    public bool IsReadyForConfirmation()
    {
        return Status == OrderStatus.Pending
            && _orderProducts?.Count > 0
            && IsFullyPaid()
            && !HasFailedPayments
            && !HasDisputedPayments;
    }

    public void UpdatePaymentStatusFlags(
        bool hasPendingPayments,
        bool hasActivePayments = false,
        bool hasFailedPayments = false,
        bool hasDisputedPayments = false)
    {
        HasPendingPayments = hasPendingPayments;
        HasActivePayments = hasActivePayments;
        HasFailedPayments = hasFailedPayments;
        HasDisputedPayments = hasDisputedPayments;
    }

    public void UpdateRefundStatus(RefundReason refundReason, Money totalRefundedAmount)
    {
        RefundReason = refundReason;
        TotalRefundedAmount = new Money(totalRefundedAmount.Amount, totalRefundedAmount.Currency);

        if (IsFullyRefunded())
        {
            RaiseDomainEvent(new OrderFullyRefundedDomainEvent(Id, ClientId, totalRefundedAmount));
        }
    }

    public bool IsFullyRefunded()
    {
        return TotalRefundedAmount.Amount >= PaidAmount.Amount;
    }

    public Result MarkAsFailedDueToPayment()
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        Status = OrderStatus.Failed;
        RaiseDomainEvent(new OrderFailedDomainEvent(Id, "Payment failed"));
        return Result.Success();
    }

    public Result MarkAsUnderReview()
    {
        if (Status is not (OrderStatus.Confirmed or OrderStatus.Processing))
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        Status = OrderStatus.UnderReview;
        RaiseDomainEvent(new OrderUnderReviewDomainEvent(Id, "Payment disputed"));
        return Result.Success();
    }

    public void UpdatePaidAmount(Money paidAmount)
    {
        PaidAmount = paidAmount;
    }
}
