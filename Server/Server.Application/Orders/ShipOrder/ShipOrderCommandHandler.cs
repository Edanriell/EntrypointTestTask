using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;

namespace Server.Application.Orders.ShipOrder;

internal sealed class ShipOrderCommandHandler : ICommandHandler<ShipOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShipOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        // ✅ Get the order
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // ✅ Create tracking number value object
        Result<TrackingNumber> trackingNumberResult = TrackingNumber.Create(request.TrackingNumber);
        if (trackingNumberResult.IsFailure)
        {
            return Result.Failure(trackingNumberResult.Error);
        }

        // ✅ Ship order with courier enum and estimated delivery date
        Result shipResult = order.Ship(
            trackingNumberResult.Value,
            request.Courier,
            request.EstimatedDeliveryDate
        );
        if (shipResult.IsFailure)
        {
            return Result.Failure(shipResult.Error);
        }

        // ✅ Update repository
        _orderRepository.Update(order);

        // ✅ Save changes using UnitOfWork
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
