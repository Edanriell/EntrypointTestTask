using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;

namespace Server.Application.Orders.MarkOutForDelivery;

internal sealed class MarkOutForDeliveryCommandHandler : ICommandHandler<MarkOutForDeliveryCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkOutForDeliveryCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkOutForDeliveryCommand request, CancellationToken cancellationToken)
    {
        // ✅ Get the order
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // ✅ Mark as out for delivery with optional updated estimated delivery date
        Result result = order.MarkOutForDelivery(request.EstimatedDeliveryDate);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        // ✅ Update repository
        _orderRepository.Update(order);

        // ✅ Save changes using UnitOfWork
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
