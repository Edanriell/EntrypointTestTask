using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments.Events;
using Server.Domain.Shared;

namespace Server.Domain.Payments;

public sealed class PaymentService
{
    private readonly OrderPaymentService _orderPaymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        OrderPaymentService orderPaymentService,
        IUnitOfWork unitOfWork
    )
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _orderPaymentService = orderPaymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ProcessAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        // Get the payment
        Payment? payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }

        Order? order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // Process the payment - the domain handles all business logic
        Result processResult = payment.Process();
        if (processResult.IsFailure)
        {
            return Result.Failure(processResult.Error);
        }

        // Update order total paid amount if only it is not 
        // expired, failed or pending! 
        order.UpdateOrderTotalPaidAmount(payment.Amount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<Payment>> AddPaymentAsync(
        Guid orderId,
        Money amount,
        PaymentMethod method,
        CancellationToken cancellationToken = default)
    {
        // Get Order
        Order? order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure<Payment>(OrderErrors.NotFound);
        }

        // Validate basic order constraints (synchronous, within Order aggregate)
        Result basicValidation = order.ValidatePayment(amount);
        if (basicValidation.IsFailure)
        {
            return Result.Failure<Payment>(basicValidation.Error);
        }

        // Comprehensive validation using domain service
        Result comprehensiveValidation = await ValidatePaymentAsync(orderId, amount, cancellationToken);
        if (comprehensiveValidation.IsFailure)
        {
            return Result.Failure<Payment>(comprehensiveValidation.Error);
        }

        // Create and save Payment
        Result<Payment> paymentResult = Payment.Create(orderId, amount, method);
        if (paymentResult.IsFailure)
        {
            return Result.Failure<Payment>(paymentResult.Error);
        }

        Payment payment = paymentResult.Value;

        _paymentRepository.Add(payment);

        // Update Order aggregate
        Result recordResult = order.RecordOrderPayment(payment.Id, amount);
        if (recordResult.IsFailure)
        {
            return Result.Failure<Payment>(recordResult.Error);
        }

        return Result.Success(payment);
    }

// Separate validation method
    private async Task<Result> ValidatePaymentAsync(
        Guid orderId,
        Money amount,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // Check if there are already pending payments, if there is we cannot create another one!
        bool hasPending = await _orderPaymentService.HasPendingPaymentsAsync(orderId, cancellationToken);
        if (hasPending)
        {
            return Result.Failure(PaymentErrors.PendingPaymentExists);
        }

        // Get outstanding amount
        Result<Money> outstandingResult = await _orderPaymentService.GetOutstandingAmountAsync(
            orderId,
            order.TotalAmount, // We'll get the actual order total in the method
            cancellationToken);
        if (outstandingResult.IsFailure)
        {
            return Result.Failure(outstandingResult.Error);
        }

        // Validate payment doesn't exceed the outstanding amount
        if (amount > outstandingResult.Value)
        {
            return Result.Failure(OrderErrors.PaymentAmountExceedsRemainingAmount);
        }

        return Result.Success();
    }

    public async Task<Result> UpdatePaymentStatusAsync(
        Guid paymentId,
        PaymentStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        // ✅ 1. Get Payment aggregate (owns the status)
        Payment? payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }

        PaymentStatus oldStatus = payment.PaymentStatus;
        Guid orderId = payment.OrderId;

        // ✅ 2. Update payment status using Payment aggregate
        Result statusUpdateResult = payment.UpdateStatus(newStatus);
        if (statusUpdateResult.IsFailure)
        {
            return statusUpdateResult;
        }

        // ✅ 3. Get Order aggregate for coordination
        Order? order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // ✅ 4. Update Order's payment status flags using domain service
        await UpdateOrderPaymentStatusFlagsAsync(order, cancellationToken);

        // ✅ 5. Handle Order status changes based on payment status
        Result orderStatusResult = await HandleOrderStatusChangeAsync(order, newStatus, cancellationToken);
        if (orderStatusResult.IsFailure)
        {
            return orderStatusResult;
        }

        // ✅ 6. Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ✅ 7. Raise domain event (Payment already raises its own events)
        payment.RaiseDomainEvent(new PaymentStatusChangedDomainEvent(orderId, paymentId, oldStatus, newStatus));

        return Result.Success();
    }

    private async Task UpdateOrderPaymentStatusFlagsAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        // ✅ Use domain service to get payment status information
        Task<bool>[] tasks = new[]
        {
            _orderPaymentService.HasPendingPaymentsAsync(order.Id, cancellationToken),
            _orderPaymentService.HasActivePaymentsAsync(order.Id, cancellationToken),
            _orderPaymentService.HasFailedPaymentsAsync(order.Id, cancellationToken)
        };

        bool[] results = await Task.WhenAll(tasks);

        // ✅ Update Order's internal state
        order.UpdatePaymentStatusFlags(
            results[0],
            results[1],
            results[2],
            results[3]);
    }

    private async Task<Result> HandleOrderStatusChangeAsync(
        Order order,
        PaymentStatus newPaymentStatus,
        CancellationToken cancellationToken)
    {
        // ✅ Handle different payment status scenarios
        return newPaymentStatus switch
        {
            PaymentStatus.Paid => await HandlePaymentPaidAsync(order, cancellationToken),
            PaymentStatus.Failed => await HandlePaymentFailedAsync(order, cancellationToken),
            PaymentStatus.Refunded => await HandlePaymentRefundedAsync(order, cancellationToken),
            _ => Result.Success()
        };
    }

    private async Task<Result> HandlePaymentPaidAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        // ✅ Check if order is now fully paid
        bool isFullyPaid = await _orderPaymentService.IsFullyPaidAsync(
            order.Id,
            order.TotalAmount,
            cancellationToken);

        if (isFullyPaid && order.Status == OrderStatus.Pending)
        {
            return order.Confirm();
        }

        return Result.Success();
    }

    private async Task<Result> HandlePaymentFailedAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        // ✅ Check if all payments have failed
        bool hasSuccessfulPayments = await _orderPaymentService.HasSuccessfulPaymentsAsync(
            order.Id,
            cancellationToken);

        if (!hasSuccessfulPayments && order.Status == OrderStatus.Pending)
        {
            return order.MarkAsFailedDueToPayment();
        }

        return Result.Success();
    }

    private async Task<Result> HandlePaymentRefundedAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        // ✅ Check if all payments are refunded
        Result<Money> netPaidResult = await _orderPaymentService.GetNetPaidAmountAsync(
            order.Id,
            order.TotalAmount.Currency,
            cancellationToken);

        Result<RefundReason> refundReasonResult = RefundReason.Create("Customer requested a refund.");
        if (refundReasonResult.IsFailure)
        {
            return Result.Failure(refundReasonResult.Error);
        }

        if (netPaidResult.IsSuccess && netPaidResult.Value.Amount == 0)
        {
            return order.MarkAsReturnedDueToRefund(refundReasonResult.Value);
        }

        return Result.Success();
    }

    private Result HandlePaymentDisputed()
    {
        // ✅ For disputed payments, we don't change the order status
        // The order can continue its normal flow while the dispute is handled separately
        // Just raise a domain event for notification purposes

        // This could trigger notifications to admins, customer service, etc.
        // but doesn't affect the order's main workflow

        return Result.Success();
    }
}
