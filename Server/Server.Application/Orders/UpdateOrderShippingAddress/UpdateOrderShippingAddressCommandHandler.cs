using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Shared;

namespace Server.Application.Orders.UpdateOrderShippingAddress;

internal sealed class UpdateOrderShippingAddressCommandHandler : ICommandHandler<UpdateOrderShippingAddressCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderShippingAddressCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateOrderShippingAddressCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
        {
            return Result.Failure(OrderErrors.CannotChangeShippingAddressAfterProcessing);
        }

        var newAddress = new Address(
            request.Country,
            request.City,
            request.ZipCode,
            request.Street
        );

        // Check if the new address is different from the current address
        if (order.ShippingAddress.Equals(newAddress))
        {
            return Result.Success();
        }

        // Update shipping address
        Result updateResult = order.UpdateShippingAddress(newAddress);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
