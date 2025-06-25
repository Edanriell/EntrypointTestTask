using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Application.Payments.RefundPaymentWithOrderUpdate;

internal sealed class RefundPaymentWithOrderUpdateCommandHandler : ICommandHandler<RefundPaymentWithOrderUpdateCommand>
{
    private readonly OrderPaymentService _orderPaymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefundPaymentWithOrderUpdateCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        OrderPaymentService orderPaymentService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _orderPaymentService = orderPaymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RefundPaymentWithOrderUpdateCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        Payment? payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure(PaymentErrors.PaymentNotFound);
        }

        // Validate payment can be refunded
        if (payment.PaymentStatus != PaymentStatus.Paid)
        {
            return Result.Failure(PaymentErrors.CannotRefundUnpaidPayment);
        }

        // Create a Money value object for refund amount
        Result<Money> refundAmountResult = Money.Create(request.RefundAmount, Currency.Eur);
        if (refundAmountResult.IsFailure)
        {
            return Result.Failure(refundAmountResult.Error);
        }

        Money refundAmount = refundAmountResult.Value;

        // Validate refund amount doesn't exceed the paid amount
        if (refundAmount > payment.PaidAmount)
        {
            return Result.Failure(PaymentErrors.RefundAmountExceedsPaidAmount);
        }

        // Here could be more complex logic encapsulated, we could use enum for
        // valid reasons for refund, but this logic is overkill for such a simple app.
        // EXAMPLE
        // if (!Enum.TryParse<RefundReason>(request.RefundReason, true, out RefundReason refundReason))
        // {
        //     return Result.Failure(PaymentErrors.InvalidRefundReason);
        // }
        Result<RefundReason> refundReasonResult = RefundReason.Create(request.RefundReason);
        if (refundReasonResult.IsFailure)
        {
            return Result.Failure(PaymentErrors.InvalidRefundReason);
        }

        // Process refund with automatic order update using domain service
        Result refundResult = _orderPaymentService.RefundPaymentWithOrderUpdate(
            payment,
            order,
            refundAmount,
            refundReasonResult.Value);

        if (refundResult.IsFailure)
        {
            return Result.Failure(refundResult.Error);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
