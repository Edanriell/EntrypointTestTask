using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Products.UpdateProductPrice;

internal sealed class UpdateProductPriceCommandHandler : ICommandHandler<UpdateProductPriceCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductPriceCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateProductPriceCommand request,
        CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        Result<Money> newPriceResult = Money.Create(request.NewPrice, Currency.Eur);
        if (newPriceResult.IsFailure)
        {
            return Result.Failure(newPriceResult.Error);
        }

        Result updatePriceResult = product.UpdatePrice(newPriceResult.Value);
        if (updatePriceResult.IsFailure)
        {
            return updatePriceResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
