using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Products;

public sealed class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(
        IProductRepository productRepository
    )
    {
        _productRepository = productRepository;
    }

    public async Task<Result> ReserveStockAsync(Guid productId, int quantity)
    {
        Product? product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        Result<Quantity> quantityResult = Quantity.CreateQuantity(quantity);
        if (quantityResult.IsFailure)
        {
            return Result.Failure(quantityResult.Error);
        }

        Result quantityToReserveResult = product.ReserveStock(quantityResult.Value);
        if (quantityToReserveResult.IsFailure)
        {
            return Result.Failure(quantityToReserveResult.Error);
        }

        return Result.Success();
    }

    public async Task<Result> ReleaseReservedStockAsync(Guid productId, int quantity)
    {
        Product? product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        Result<Quantity> quantityResult = Quantity.CreateQuantity(quantity);
        if (quantityResult.IsFailure)
        {
            return Result.Failure(quantityResult.Error);
        }

        Result quantityToReleaseResult = product.ReleaseReservedStock(quantityResult.Value);
        if (quantityToReleaseResult.IsFailure)
        {
            return Result.Failure(quantityToReleaseResult.Error);
        }

        return Result.Success();
    }
}
