using Server.Domain.Abstractions;
using Server.Domain.OrderItems;
using Server.Domain.Orders.Events;
using Server.Domain.Payments;
using Server.Domain.Shared;
using Server.Domain.Users;

namespace Server.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderProduct>? _orderProducts = new();

    // MANY PAYMENTS
    private readonly List<Payment> _payments = new();

    private Order(
        Guid id,
        Guid clientId,
        // MANY PAYMENTS
        // Guid? paymentId,
        OrderNumber orderNumber,
        Address shippingAddress,
        DateTime createdAt,
        OrderStatus orderStatus
    ) : base(id)
    {
        ClientId = clientId;
        // MANY PAYMENTS
        // PaymentId = paymentId;
        OrderNumber = orderNumber;
        ShippingAddress = shippingAddress;
        CreatedAt = createdAt;
        Status = orderStatus;
    }

    private Order() { }

    public Guid ClientId { get; }

    // MANY PAYMENTS
    // public Guid? PaymentId { get; private set; }
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
    public TrackingNumber TrackingNumber { get; private set; }
    public decimal TotalAmount => CalculateTotal().Amount;

    // Navigation property
    public User Client { get; private set; }

    // MANY PAYMENTS
    // public Payment? Payment { get; private set; }
    public IReadOnlyCollection<Payment> Payments => _payments?.AsReadOnly() ?? new List<Payment>().AsReadOnly();
    public IReadOnlyCollection<OrderProduct> OrderProducts => _orderProducts?.AsReadOnly();
    public bool CanBeShipped => Status == OrderStatus.Confirmed;

    public static Result<Order> Create(
        Guid clientId,
        OrderNumber orderNumber,
        Address shippingAddress)
    {
        var order = new Order(
            Guid.NewGuid(),
            clientId,
            // MANY PAYMENTS
            // null,
            orderNumber,
            shippingAddress,
            DateTime.UtcNow,
            OrderStatus.Pending
        );

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, order.ClientId));

        return Result.Success(order);
    }

    public Result AddProduct(
        OrderProduct orderProduct
        // Guid productId,
        // ProductName productName,
        // Money unitPrice,
        // Quantity quantity
    )
    {
        if (Status is not OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        OrderProduct? existingProduct = _orderProducts?.FirstOrDefault(op => op.ProductId == orderProduct.ProductId);
        if (existingProduct is not null)
        {
            return existingProduct.UpdateQuantity(existingProduct.Quantity + orderProduct.Quantity);
        }

        _orderProducts?.Add(orderProduct);
        RecalculateTotal();

        return Result.Success();

        // if (Status is not OrderStatus.Pending)
        // {
        //     return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        // }
        //
        // if (quantity.Value <= 0)
        // {
        //     return Result.Failure(OrderErrors.InvalidQuantity);
        // }
        //
        // OrderProduct? existingProduct = _orderProducts.FirstOrDefault(op => op.ProductId == productId);
        // if (existingProduct is not null)
        // {
        //     return existingProduct.UpdateQuantity(existingProduct.Quantity + quantity);
        // }
        //
        // Result<OrderProduct> newOrderProduct = OrderProduct.Create(
        //     Id,
        //     productId,
        //     productName,
        //     unitPrice,
        //     quantity
        // );
        //
        // if (newOrderProduct.IsFailure)
        // {
        //     return Result.Failure(newOrderProduct.Error);
        // }
        //
        // _orderProducts.Add(newOrderProduct.Value);
        // RecalculateTotal();
        //
        // return Result.Success();
    }

    // MANY PAYMENTS
    public Result AddPayment(Payment payment)
    {
        if (Status == OrderStatus.Cancelled || Status == OrderStatus.Returned)
        {
            return Result.Failure(OrderErrors.CannotAddPaymentToInactiveOrder);
        }

        _payments?.Add(payment);
        return Result.Success();
    }

    // Pass Currency as argument if multiple
    // currencies are supported
    public Money GetTotalPaidAmount()
    {
        if (_payments == null || !_payments.Any())
        {
            return Money.Zero(); // or Money.Zero(Currency.Default) if you have a default currency
        }

        return _payments
            .Where(p => p.PaymentStatus == PaymentStatus.Paid)
            .Select(p => p.Amount)
            .Aggregate(Money.Zero(), (total, amount) => total + amount);
    }


    public Money GetTotalOutstandingAmount()
    {
        Money orderTotal = CalculateTotal();
        Money totalPaid = GetTotalPaidAmount();

        return new Money(orderTotal.Amount - totalPaid.Amount, orderTotal.Currency);
    }

    public bool IsFullyPaid()
    {
        Money orderTotal = CalculateTotal();
        Money totalPaid = GetTotalPaidAmount();

        return totalPaid.Amount >= orderTotal.Amount;
    }

    public bool HasPartialPayment()
    {
        Money totalPaid = GetTotalPaidAmount();
        return totalPaid.Amount > 0 && !IsFullyPaid();
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
        RecalculateTotal();

        if (_orderProducts is not { Count: > 0 })
        {
            return Result.Failure(OrderErrors.CannotRemoveLastProduct);
        }

        return Result.Success();
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

        RecalculateTotal();
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

    public Result ConfirmOrder()
    {
        if (Status is not OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        if (_orderProducts is not { Count: > 0 })
        {
            return Result.Failure(OrderErrors.EmptyOrder);
        }

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        RaiseDomainEvent(new OrderConfirmedDomainEvent(Id, ClientId, CalculateTotal()));

        return Result.Success();
    }


    public Result ConfirmWithoutPaymentCheck()
    {
        if (Status is not OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        if (_orderProducts is not { Count: > 1 })
        {
            return Result.Failure(OrderErrors.EmptyOrder);
        }

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        RaiseDomainEvent(new OrderConfirmedDomainEvent(Id, ClientId, CalculateTotal()));

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

    // public Result Ship(TrackingNumber trackingNumber)
    // {
    //     if (Status != OrderStatus.Processing)
    //     {
    //         return Result.Failure(OrderErrors.InvalidStatusTransition);
    //     }
    //
    //     if (string.IsNullOrWhiteSpace(trackingNumber.Value))
    //     {
    //         return Result.Failure(OrderErrors.InvalidTrackingNumber);
    //     }
    //
    //     Status = OrderStatus.Shipped;
    //     ShippedAt = DateTime.UtcNow;
    //     TrackingNumber = trackingNumber;
    //
    //     RaiseDomainEvent(new OrderShippedDomainEvent(Id));
    //
    //     return Result.Success();
    // }

    // public Result MarkAsDelivered()
    // {
    //     if (Status != OrderStatus.Shipped)
    //     {
    //         return Result.Failure(OrderErrors.InvalidStatusTransition);
    //     }
    //
    //     Status = OrderStatus.Delivered;
    //     DeliveredAt = DateTime.UtcNow;
    //
    //     RaiseDomainEvent(new OrderDeliveredDomainEvent(Id, ClientId));
    //
    //     return Result.Success();
    // }

    // public Result Cancel(CancellationReason reason)
    // {
    //     if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Returned)
    //     {
    //         return Result.Failure(OrderErrors.CannotCancelShippedOrder);
    //     }
    //
    //     Status = OrderStatus.Cancelled;
    //     CancelledAt = DateTime.UtcNow;
    //     CancellationReason = reason;
    //
    //     RaiseDomainEvent(new OrderCancelledDomainEvent(Id, Status, reason));
    //
    //     return Result.Success();
    // }

    // public Result Return(ReturnReason reason)
    // {
    //     if (Status != OrderStatus.Delivered)
    //     {
    //         return Result.Failure(OrderErrors.CanOnlyReturnDeliveredOrders);
    //     }
    //
    //     int daysSinceDelivery = (DateTime.UtcNow - DeliveredAt!.Value).Days;
    //     if (daysSinceDelivery > 30)
    //     {
    //         return Result.Failure(OrderErrors.ReturnWindowExpired);
    //     }
    //
    //     Status = OrderStatus.Returned;
    //     RaiseDomainEvent(new OrderReturnedDomainEvent(Id, ClientId, reason));
    //
    //     return Result.Success();
    // }

    public Result MarkAsReturnedDueToRefund(RefundReason refundReason)
    {
        if (Status is OrderStatus.Cancelled or OrderStatus.Returned)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        if (Status is not (OrderStatus.Delivered or OrderStatus.Confirmed or OrderStatus.Shipped))
        {
            return Result.Failure(OrderErrors.CannotReturnOrderInCurrentStatus);
        }

        Status = OrderStatus.Returned;
        ReturnReason = ReturnReason.Create(refundReason.Value);

        RaiseDomainEvent(new OrderReturnedDomainEvent(Id, ClientId, ReturnReason));

        return Result.Success();
    }

    // Important !
    // Ideally, if our system supports many currencies,
    // we need to create parameter Currency currency and pass it to Aggregate
    // this is necessary, for propper reduce
    public Money CalculateTotal()
    {
        return _orderProducts?
            .Select(op => op.TotalPrice)
            .Aggregate(Money.Zero(Currency.Eur), (total, price) => total + price);
    }
    // public Money CalculateTotal()
    // {
    //     if (!OrderProducts.Any())
    //     {
    //         return Money.Zero(Currency.Eur);
    //     }
    //
    //     // Get currency from first product
    //     Currency currency = OrderProducts.First().TotalPrice.Currency;
    //
    //     // Sum all total prices
    //     decimal totalAmount = OrderProducts.Sum(op => op.TotalPrice.Amount);
    //
    //     return new Money(totalAmount, currency);
    // }


    private void RecalculateTotal()
    {
        Money newTotal = CalculateTotal();
        RaiseDomainEvent(new OrderTotalChangedDomainEvent(Id, newTotal));
    }

    public bool HasProduct(Guid productId)
    {
        return _orderProducts?.Any(op => op.ProductId == productId) ?? false;
    }

    public Result<Quantity> GetProductQuantity(Guid productId)
    {
        return _orderProducts?.FirstOrDefault(op => op.ProductId == productId)?.Quantity ?? Quantity.CreateQuantity(0);
    }

    public Result<Money> GetProductTotal(Guid productId)
    {
        return _orderProducts?.FirstOrDefault(op => op.ProductId == productId)?.TotalPrice ?? Money.Zero();
    }

    // MANY PAYMENTS
    // public Result SetPaymentId(Guid paymentId)
    // {
    //     // Check for invalid order status (optional, adjust as needed)
    //     if (Status == OrderStatus.Cancelled || Status == OrderStatus.Returned)
    //     {
    //         return Result.Failure(OrderErrors.CannotAssignPaymentToInactiveOrder);
    //     }
    //
    //     // Prevent reassignment if already assigned
    //     if (PaymentId.HasValue)
    //     {
    //         return Result.Failure(OrderErrors.PaymentAlreadyAssigned);
    //     }
    //
    //     PaymentId = paymentId;
    //     return Result.Success();
    // }

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

    public Result Ship(TrackingNumber trackingNumber)
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

        RaiseDomainEvent(new OrderShippedDomainEvent(Id));

        return Result.Success();
    }

    public Result MarkOutForDelivery()
    {
        if (Status != OrderStatus.Shipped)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

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

    public Result CompleteOrder()
    {
        if (Status != OrderStatus.Delivered)
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        // Check if return window has expired (e.g., 30 days)
        int daysSinceDelivery = (DateTime.UtcNow - DeliveredAt!.Value).Days;
        if (daysSinceDelivery < 30)
        {
            return Result.Failure(OrderErrors.CannotCompleteOrderWithinReturnWindow);
        }

        Status = OrderStatus.Completed;
        RaiseDomainEvent(new OrderCompletedDomainEvent(Id, ClientId));

        return Result.Success();
    }

    public Result Cancel(CancellationReason reason)
    {
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
        if (Status != OrderStatus.Delivered)
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
}
