using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.OrderProducts;
using Server.Domain.Orders;
using Server.Domain.Products;

namespace Server.Application.Orders.CompleteOrder;

// TODO22 Needs testing !
internal sealed class CompleteOrderCommandHandler : ICommandHandler<CompleteOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteOrderCommandHandler(
        IOrderRepository orderRepository,
        ProductService productService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        // ✅ Get order with products to release stock properly
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // ✅ Complete the order first
        Result completeResult = order.Complete();
        if (completeResult.IsFailure)
        {
            return completeResult;
        }

        // ✅ Release reserved stock for all products in the order
        foreach (OrderProduct orderProduct in order.OrderProducts)
        {
            Result releaseResult = await _productService.ReleaseReservedStockAsync(
                orderProduct.ProductId,
                orderProduct.Quantity.Value
            );

            if (releaseResult.IsFailure)
            {
                return releaseResult;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
