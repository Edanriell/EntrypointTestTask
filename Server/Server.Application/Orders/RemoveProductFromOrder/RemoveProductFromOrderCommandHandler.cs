using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Orders.RemoveProductFromOrder;

internal sealed class RemoveProductFromOrderCommandHandler : ICommandHandler<RemoveProductFromOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProductFromOrderCommandHandler(
        IOrderRepository orderRepository,
        ProductService productService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveProductFromOrderCommand request,
        CancellationToken cancellationToken)
    {
        // ✅ Get order with products
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        if (order.Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        var stockToRelease = new List<(Guid ProductId, int Quantity)>();

        // ✅ Process each product removal request
        foreach (ProductRemovalRequest removal in request.ProductRemovals)
        {
            if (!order.HasProduct(removal.ProductId))
            {
                return Result.Failure(OrderErrors.ProductNotFound);
            }

            // Get current quantity in order
            Result<Quantity> currentQuantityResult = order.GetProductQuantity(removal.ProductId);
            if (currentQuantityResult.IsFailure)
            {
                return Result.Failure(currentQuantityResult.Error);
            }

            int currentQuantity = currentQuantityResult.Value.Value;
            int quantityToRemove = removal.Quantity ?? currentQuantity; // null = remove all

            // ✅ Validate removal quantity
            if (quantityToRemove <= 0)
            {
                return Result.Failure(OrderErrors.InvalidQuantityToRemove);
            }

            if (quantityToRemove > currentQuantity)
            {
                return Result.Failure(OrderErrors.CannotRemoveMoreThanAvailable);
            }

            // ✅ Track stock to release
            stockToRelease.Add((removal.ProductId, quantityToRemove));

            // ✅ Handle full vs partial removal
            if (quantityToRemove == currentQuantity)
            {
                // Remove entire product
                Result removeResult = order.RemoveProduct(removal.ProductId);
                if (removeResult.IsFailure)
                {
                    return removeResult;
                }
            }
            else
            {
                // Reduce quantity
                int newQuantity = currentQuantity - quantityToRemove;
                Result<Quantity> newQuantityResult = Quantity.CreateQuantity(newQuantity);
                if (newQuantityResult.IsFailure)
                {
                    return Result.Failure(newQuantityResult.Error);
                }

                Result updateResult = order.UpdateProductQuantity(removal.ProductId, newQuantityResult.Value);
                if (updateResult.IsFailure)
                {
                    return updateResult;
                }
            }
        }

        // ✅ Release reserved stock using ProductService
        foreach ((Guid productId, int quantity) in stockToRelease)
        {
            Result releaseResult = await _productService.ReleaseReservedStockAsync(productId, quantity);
            if (releaseResult.IsFailure)
            {
                return releaseResult;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
