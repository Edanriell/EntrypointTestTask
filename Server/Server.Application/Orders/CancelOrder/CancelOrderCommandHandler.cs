using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.OrderProducts;
using Server.Domain.Orders;
using Server.Domain.Products;

namespace Server.Application.Orders.CancelOrder;

internal sealed class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        ProductService productService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        // ✅ Get order with products to release reserved stock
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // ✅ Create cancellation reason
        Result<CancellationReason> cancellationReasonResult = CancellationReason.Create(request.CancellationReason);
        if (cancellationReasonResult.IsFailure)
        {
            return Result.Failure(cancellationReasonResult.Error);
        }

        // ✅ Cancel the order first
        Result cancelResult = order.Cancel(cancellationReasonResult.Value);
        if (cancelResult.IsFailure)
        {
            return Result.Failure(cancelResult.Error);
        }

        // ✅ Release reserved stock for all products in the cancelled order
        foreach (OrderProduct orderProduct in order.OrderProducts)
        {
            Result releaseResult = await _productService.ReleaseReservedStockAsync(
                orderProduct.ProductId,
                orderProduct.Quantity.Value
            );

            if (releaseResult.IsFailure)
            {
                return Result.Failure(releaseResult.Error);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
