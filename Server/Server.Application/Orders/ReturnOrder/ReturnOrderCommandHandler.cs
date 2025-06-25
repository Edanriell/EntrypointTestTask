using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;

namespace Server.Application.Orders.ReturnOrder;

internal sealed class ReturnOrderCommandHandler : ICommandHandler<ReturnOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReturnOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ReturnOrderCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        Result<ReturnReason> returnReasonResult = ReturnReason.Create(request.ReturnReason);
        if (returnReasonResult.IsFailure)
        {
            return Result.Failure(returnReasonResult.Error);
        }

        Result result = order.Return(
            returnReasonResult.Value);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
