using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Refunds;
using Server.Domain.Shared;

namespace Server.Domain.Payments;

public sealed class OrderPaymentService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IRefundRepository _refundRepository;

    public OrderPaymentService(
        IPaymentRepository paymentRepository, IOrderRepository orderRepository, IRefundRepository refundRepository)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _refundRepository = refundRepository;
    }

    public async Task<Result> ProcessFullRefundForOrderAsync(
        Guid orderId,
        RefundReason reason,
        CancellationToken cancellationToken = default)
    {
        // Get Order aggregate
        Order? order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // Validate Order status
        if (order.Status is not (OrderStatus.Cancelled or OrderStatus.Returned))
        {
            return Result.Failure(OrderErrors.CanOnlyRefundReturnedOrCancelledOrders);
        }

        // Get all payments for this order using repository
        IReadOnlyList<Payment> payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        // Filter payments that can be refunded
        var paidPayments = payments
            .Where(p => p.PaymentStatus is PaymentStatus.Paid)
            .ToList();

        if (!paidPayments.Any())
        {
            return Result.Failure(PaymentErrors.NoPaymentsToRefund);
        }

        // Process refunds for all paid payments
        foreach (Payment payment in paidPayments)
        {
            Result<Refund> refundResult = Refund.Create(payment.Id, payment.Amount.Amount, reason);
            if (refundResult.IsFailure)
            {
                return Result.Failure<Refund>(refundResult.Error);
            }

            // Important!
            // Is it approach valid?
            _refundRepository.Add(refundResult.Value);
            // Do we need attach entity manually?
            refundResult.Value.AttachToPayment(payment);

            Result paymentProcessResult = payment.ProcessRefund(refundResult.Value.Amount, reason);
            if (paymentProcessResult.IsFailure)
            {
                return Result.Failure(paymentProcessResult.Error);
            }
        }

        // Update Order's refund
        Result<Money> totalRefundedResult =
            await GetTotalRefundedAmountAsync(orderId, order.Currency, cancellationToken);

        if (totalRefundedResult.IsSuccess)
        {
            order.UpdateRefundStatus(reason, totalRefundedResult.Value);
        }

        return Result.Success();
    }

    public async Task<Result<Money>> GetTotalPaidAmountAsync(
        Guid orderId,
        Currency currency,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Payment> payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        var paidPayments = payments
            .Where(p => p.PaymentStatus == PaymentStatus.Paid)
            .ToList();

        if (!paidPayments.Any())
        {
            return Result.Success(Money.Zero(currency));
        }

        // Ensure all payments are in the same currency
        if (paidPayments.Any(p => p.Amount.Currency != currency))
        {
            return Result.Failure<Money>(PaymentErrors.CurrencyMismatch);
        }

        decimal totalAmount = paidPayments.Sum(p => p.Amount.Amount);
        var totalMoney = new Money(totalAmount, currency);

        return Result.Success(totalMoney);
    }

    public async Task<Result<Money>> GetOutstandingAmountAsync(
        Guid orderId,
        Money orderTotal,
        CancellationToken cancellationToken = default)
    {
        Result<Money> paidResult = await GetTotalPaidAmountAsync(orderId, orderTotal.Currency, cancellationToken);
        if (paidResult.IsFailure)
        {
            return Result.Failure<Money>(paidResult.Error);
        }

        decimal outstandingAmount = orderTotal.Amount - paidResult.Value.Amount;
        var outstandingMoney = new Money(Math.Max(0, outstandingAmount), orderTotal.Currency);

        return Result.Success(outstandingMoney);
    }

    public async Task<bool> HasPendingPaymentsAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Payment> payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);
        return payments.Any(p => p.PaymentStatus is PaymentStatus.Pending or PaymentStatus.Processing);
    }

    public async Task<bool> IsFullyPaidAsync(
        Guid orderId,
        Money orderTotal,
        CancellationToken cancellationToken = default)
    {
        Result<Money> paidResult = await GetTotalPaidAmountAsync(orderId, orderTotal.Currency, cancellationToken);
        if (paidResult.IsFailure)
        {
            return false;
        }

        return paidResult.Value.Amount >= orderTotal.Amount;
    }

    public async Task<Result<Money>> GetNetPaidAmountAsync(
        Guid orderId,
        Currency currency,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Payment> payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        if (!payments.Any())
        {
            return Result.Success(Money.Zero(currency));
        }

        // Ensure all payments are in the same currency
        if (payments.Any(p => p.Amount.Currency != currency))
        {
            return Result.Failure<Money>(PaymentErrors.CurrencyMismatch);
        }

        decimal totalPaidAmount = 0;
        decimal totalRefundedAmount = 0;

        foreach (Payment payment in payments)
        {
            switch (payment.PaymentStatus)
            {
                case PaymentStatus.Paid:

                case PaymentStatus.Refunded:
                    // Don't count refunded payments as paid
                    totalRefundedAmount += payment.Amount.Amount;
                    break;
            }
        }

        decimal netAmount = totalPaidAmount - totalRefundedAmount;
        return Result.Success(new Money(Math.Max(0, netAmount), currency));
    }

    public async Task<bool> HasFailedPaymentsAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Payment> payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return payments.Any(p => p.PaymentStatus == PaymentStatus.Failed);
    }

    public async Task<bool> HasActivePaymentsAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Payment> payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return payments.Any(p => p.PaymentStatus is
            PaymentStatus.Pending or
            PaymentStatus.Processing or
            PaymentStatus.Paid);
    }

    public async Task<bool> HasSuccessfulPaymentsAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Payment> payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return payments.Any(p => p.PaymentStatus is
            PaymentStatus.Paid);
    }

    public async Task<Result<Money>> GetTotalRefundedAmountAsync(
        Guid orderId,
        Currency currency,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Payment> payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        if (!payments.Any())
        {
            return Result.Success(Money.Zero(currency));
        }

        // Ensure all payments are in the same currency
        if (payments.Any(p => p.Amount.Currency != currency))
        {
            return Result.Failure<Money>(PaymentErrors.CurrencyMismatch);
        }

        decimal totalRefunded = 0;

        foreach (Payment payment in payments)
        {
            if (payment.PaymentStatus is PaymentStatus.Refunded)
            {
                totalRefunded += payment.GetRefundedAmount().Amount;
            }
        }

        return Result.Success(new Money(totalRefunded, currency));
    }
}
