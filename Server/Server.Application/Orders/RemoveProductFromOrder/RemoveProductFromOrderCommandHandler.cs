using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Orders.RemoveProductFromOrder;

// This commands needs testing !
internal sealed class RemoveProductFromOrderCommandHandler : ICommandHandler<RemoveProductFromOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProductFromOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveProductFromOrderCommand request,
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

        var productsToReleaseStock = new List<(Guid ProductId, int Quantity)>();
        foreach (Guid productId in request.ProductIds)
        {
            if (!order.HasProduct(productId))
            {
                return Result.Failure(OrderErrors.ProductNotFound);
            }

            // Get the current product quantity before removal (for stock release)
            Result<Quantity> quantityResult = order.GetProductQuantity(productId);
            if (quantityResult.IsFailure)
            {
                return Result.Failure(quantityResult.Error);
            }

            productsToReleaseStock.Add((productId, quantityResult.Value.Value));

            // Remove the product from order
            Result removeResult = order.RemoveProduct(productId);
            if (removeResult.IsFailure)
            {
                return Result.Failure(removeResult.Error);
            }
        }

        // Release reserved stock for removed products
        foreach ((Guid productId, int quantity) in productsToReleaseStock)
        {
            Product? product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is not null)
            {
                // Release the reserved stock
                Result<Quantity> quantityToRelease = Quantity.CreateQuantity(quantity);
                if (quantityToRelease.IsSuccess)
                {
                    product.UpdateReservedStock(quantityToRelease.Value);
                }
            }
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
