using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments;

namespace Server.Application.Payments.ConfirmOrderWithPaymentCheck;

internal sealed class ConfirmOrderWithPaymentCheckCommandHandler : ICommandHandler<ConfirmOrderWithPaymentCheckCommand>
{
    private readonly OrderPaymentService _orderPaymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmOrderWithPaymentCheckCommandHandler(
        IOrderRepository orderRepository,
        OrderPaymentService orderPaymentService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _orderPaymentService = orderPaymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConfirmOrderWithPaymentCheckCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // Check if order can be confirmed with current payments
        Result canConfirmResult = _orderPaymentService.CanConfirmOrderWithPayments(order);
        if (canConfirmResult.IsFailure)
        {
            return canConfirmResult;
        }

        // Confirm the order
        Result confirmResult = order.ConfirmOrder();
        if (confirmResult.IsFailure)
        {
            return confirmResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
