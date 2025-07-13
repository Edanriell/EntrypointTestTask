using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments;

namespace Server.Application.Payments.ProcessFullRefundForOrder;

internal sealed class ProcessFullRefundForOrderCommandHandler : ICommandHandler<ProcessFullRefundForOrderCommand>
{
    private readonly OrderPaymentService _orderPaymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessFullRefundForOrderCommandHandler(
        IOrderRepository orderRepository,
        OrderPaymentService orderPaymentService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _orderPaymentService = orderPaymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ProcessFullRefundForOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        Result<RefundReason> reasonResult = RefundReason.Create(request.Reason);
        if (reasonResult.IsFailure)
        {
            return Result.Failure(reasonResult.Error);
        }

        Result refundResult = _orderPaymentService.ProcessFullRefundForOrder(order, reasonResult.Value);
        if (refundResult.IsFailure)
        {
            return refundResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
