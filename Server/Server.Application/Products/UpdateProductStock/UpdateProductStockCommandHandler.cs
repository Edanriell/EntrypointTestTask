using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Products.UpdateProductStock;

internal sealed class UpdateProductStockCommandHandler : ICommandHandler<UpdateProductStockCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductStockCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateProductStockCommand request,
        CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        Result<Quantity> stockResult = Quantity.CreateQuantity(request.Stock);
        if (stockResult.IsFailure)
        {
            return Result.Failure(stockResult.Error);
        }

        Result updateStockResult = product.UpdateStock(stockResult.Value);
        if (updateStockResult.IsFailure)
        {
            return updateStockResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
