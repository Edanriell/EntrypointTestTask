using Server.Domain.Abstractions;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Domain.OrderProducts;

public sealed class OrderProductService
{
    public async Task<Result<List<OrderProduct>>> CreateOrderProductsAsync(
        Guid orderId,
        ICollection<(Guid ProductId, Quantity Quantity)> orderItems,
        IProductRepository productRepository,
        CancellationToken cancellationToken = default)
    {
        var orderProducts = new List<OrderProduct>();

        foreach ((Guid productId, Quantity quantity) in orderItems)
        {
            Result<OrderProduct> orderProductResult = await CreateOrderProductAsync(
                orderId,
                productId,
                quantity,
                productRepository,
                cancellationToken);

            if (orderProductResult.IsFailure)
            {
                return Result.Failure<List<OrderProduct>>(orderProductResult.Error);
            }

            orderProducts.Add(orderProductResult.Value);
        }

        return Result.Success(orderProducts);
    }

    private async Task<Result<OrderProduct>> CreateOrderProductAsync(
        Guid orderId,
        Guid productId,
        Quantity quantity,
        IProductRepository productRepository,
        CancellationToken cancellationToken = default)
    {
        Product? product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return Result.Failure<OrderProduct>(ProductErrors.NotFound);
        }

        // Check stock availability
        if (!product.HasSufficientStock(quantity))
        {
            return Result.Failure<OrderProduct>(OrderProductErrors.InsufficientStock);
        }

        // Create an OrderProduct with all required data from the Product
        Result<OrderProduct> orderProductResult = OrderProduct.Create(
            orderId,
            productId,
            product.Name,
            product.Price,
            quantity
        );
        if (orderProductResult.IsFailure)
        {
            return Result.Failure<OrderProduct>(orderProductResult.Error);
        }

        // Reserve stock
        // Result reservedStockResult = product.AdjustStock(quantity);
        // if (reservedStockResult.IsFailure)
        // {
        //     return Result.Failure<OrderProduct>(reservedStockResult.Error);
        // }

        return orderProductResult;
    }
}
