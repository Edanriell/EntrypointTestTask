using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Shared;

namespace Server.Application.Orders.UpdateOrder;

internal sealed class UpdateOrderCommandHandler : ICommandHandler<UpdateOrder>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateOrder request,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        bool hasChanges = false;

        // Create new address using existing values as defaults and updating only provided fields
        var newAddress = new Address(
            request.Country ?? order.ShippingAddress.Country,
            request.City ?? order.ShippingAddress.City,
            request.ZipCode ?? order.ShippingAddress.ZipCode,
            request.Street ?? order.ShippingAddress.Street
        );

        // Check if the new address is different from the current address
        if (!order.ShippingAddress.Equals(newAddress))
        {
            Result updateAddressResult = order.UpdateShippingAddress(newAddress);
            if (updateAddressResult.IsFailure)
            {
                return Result.Failure(updateAddressResult.Error);
            }

            hasChanges = true;
        }

        // Update Info if provided
        if (!string.IsNullOrWhiteSpace(request.Info) && request.Info != order.Info?.Value)
        {
            var orderInfoResult = OrderInfo.Create(request.Info);

            Result updateInfoResult = order.UpdateInfo(orderInfoResult);
            if (updateInfoResult.IsFailure)
            {
                return Result.Failure(updateInfoResult.Error);
            }

            hasChanges = true;
        }

        // Only save if there were actual changes
        if (hasChanges)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
