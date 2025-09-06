using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Products.DiscountProduct;

internal sealed class DiscountProductCommandHandler : ICommandHandler<DiscountProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DiscountProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }
 
    public async Task<Result> Handle(
        DiscountProductCommand request,
        CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        Result<Money> newPriceResult = Money.Create(request.NewPrice, product.Price.Currency);
        if (newPriceResult.IsFailure)
        {
            return Result.Failure(newPriceResult.Error);
        }

        Result discountResult = product.Discount(newPriceResult.Value);
        if (discountResult.IsFailure)
        {
            return Result.Failure(discountResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
