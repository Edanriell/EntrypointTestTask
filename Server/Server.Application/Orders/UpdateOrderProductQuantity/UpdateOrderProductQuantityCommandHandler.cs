using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.OrderProducts;
using Server.Domain.Orders;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Orders.UpdateOrderProductQuantity;

internal sealed class UpdateOrderProductQuantityCommandHandler : ICommandHandler<UpdateOrderProductQuantityCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderProductQuantityCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateOrderProductQuantityCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        if (order.Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        if (!order.HasProduct(request.ProductId))
        {
            return Result.Failure(OrderErrors.ProductNotFound);
        }

        // Get current product quantity for stock adjustment calculation
        Result<Quantity> currentQuantityResult = order.GetProductQuantity(request.ProductId);
        if (currentQuantityResult.IsFailure)
        {
            return Result.Failure(currentQuantityResult.Error);
        }

        int currentQuantity = currentQuantityResult.Value.Value;

        Result<Quantity> newQuantityResult = Quantity.CreateQuantity(request.NewQuantity);
        if (newQuantityResult.IsFailure)
        {
            return Result.Failure(newQuantityResult.Error);
        }

        Quantity newQuantity = newQuantityResult.Value;

        // Get product for stock validation and adjustment
        Product? product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        // Calculate stock adjustment needed
        int quantityDifference = request.NewQuantity - currentQuantity;

        // Handle stock adjustment
        if (quantityDifference > 0)
        {
            // Increasing quantity - need to reserve more stock
            Result<Quantity> additionalQuantityResult = Quantity.CreateQuantity(quantityDifference);
            if (additionalQuantityResult.IsFailure)
            {
                return Result.Failure(additionalQuantityResult.Error);
            }

            // Check if sufficient stock is available
            if (!product.HasSufficientStock(additionalQuantityResult.Value))
            {
                return Result.Failure(OrderProductErrors.InsufficientStock);
            }

            // Reserve additional stock
            Result reserveResult = product.UpdateReservedStock(additionalQuantityResult.Value);
            if (reserveResult.IsFailure)
            {
                return Result.Failure(reserveResult.Error);
            }
        }
        else if (quantityDifference < 0)
        {
            // Decreasing quantity - need to release reserved stock
            int quantityToRelease = quantityDifference;
            Result<Quantity> releaseQuantityResult = Quantity.CreateQuantity(quantityToRelease);
            if (releaseQuantityResult.IsFailure)
            {
                return Result.Failure(releaseQuantityResult.Error);
            }

            Result reserveResult = product.UpdateReservedStock(releaseQuantityResult.Value);
            if (reserveResult.IsFailure)
            {
                return Result.Failure(reserveResult.Error);
            }
        }
        // If quantityDifference == 0, no stock adjustment needed

        // Update the product quantity in the order
        Result updateResult = order.UpdateProductQuantity(request.ProductId, newQuantity);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
