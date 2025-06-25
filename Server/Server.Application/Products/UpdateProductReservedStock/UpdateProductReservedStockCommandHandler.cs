using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Products.UpdateProductReservedStock;

internal sealed class UpdateProductReservedStockCommandHandler : ICommandHandler<UpdateProductReservedStockCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductReservedStockCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateProductReservedStockCommand request,
        CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        Result<Quantity> reservedStockResult = Quantity.CreateQuantity(request.ReservedStock);
        if (reservedStockResult.IsFailure)
        {
            return Result.Failure(reservedStockResult.Error);
        }

        Result updateReservedStockResult = product.UpdateReservedStock(reservedStockResult.Value);
        if (updateReservedStockResult.IsFailure)
        {
            return updateReservedStockResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
