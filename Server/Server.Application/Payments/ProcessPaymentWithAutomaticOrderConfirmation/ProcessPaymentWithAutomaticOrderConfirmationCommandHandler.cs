using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Application.Payments.ProcessPaymentWithAutomaticOrderConfirmation;

internal sealed class ProcessPaymentWithAutomaticOrderConfirmationCommandHandler
    : ICommandHandler<ProcessPaymentWithAutomaticOrderConfirmationCommand>
{
    private readonly OrderPaymentService _orderPaymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentWithAutomaticOrderConfirmationCommandHandler(
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
        ProcessPaymentWithAutomaticOrderConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // Get the payment associated with the order
        Payment? payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure(PaymentErrors.PaymentNotFound);
        }

        if (order.Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotProcessPaymentForNonPendingOrder);
        }

        // Create a new Money value object for payment amount
        Result<Money> paymentAmountResult = Money.Create(request.PaymentAmount, Currency.Eur);
        if (paymentAmountResult.IsFailure)
        {
            return Result.Failure(paymentAmountResult.Error);
        }

        Money paymentAmount = paymentAmountResult.Value;

        // Update payment total amount from order (in case order items changed)
        Result updateTotalResult = _orderPaymentService.UpdatePaymentTotalFromOrder(payment, order);
        if (updateTotalResult.IsFailure)
        {
            return Result.Failure(updateTotalResult.Error);
        }

        // Process payment with automatic order confirmation
        Result processResult = _orderPaymentService.ProcessPaymentWithAutomaticOrderConfirmation(
            payment,
            order,
            paymentAmount);

        if (processResult.IsFailure)
        {
            return Result.Failure(processResult.Error);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
