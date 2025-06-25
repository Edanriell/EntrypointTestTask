using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Get the product
        Product? product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        ProductName? productName = null;
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            Result<ProductName> nameResult = ProductName.Create(request.Name);
            if (nameResult.IsFailure)
            {
                return Result.Failure(nameResult.Error);
            }

            productName = nameResult.Value;
        }

        ProductDescription? productDescription = null;
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            Result<ProductDescription> descriptionResult = ProductDescription.Create(request.Description);
            if (descriptionResult.IsFailure)
            {
                return Result.Failure(descriptionResult.Error);
            }

            productDescription = descriptionResult.Value;
        }

        Money? price = null;
        if (request.Price.HasValue)
        {
            Result<Money> priceResult = Money.Create(request.Price.Value, Currency.Eur);
            if (priceResult.IsFailure)
            {
                return Result.Failure(priceResult.Error);
            }

            price = priceResult.Value;
        }

        Quantity? stockChange = null;
        if (request.StockChange.HasValue)
        {
            Result<Quantity> stockResult = Quantity.CreateQuantity(request.StockChange.Value);
            if (stockResult.IsFailure)
            {
                return Result.Failure(stockResult.Error);
            }

            stockChange = stockResult.Value;
        }

        Quantity? reservedChange = null;
        if (request.ReservedChange.HasValue)
        {
            Result<Quantity> reservedResult = Quantity.CreateQuantity(request.ReservedChange.Value);
            if (reservedResult.IsFailure)
            {
                return Result.Failure(reservedResult.Error);
            }

            reservedChange = reservedResult.Value;
        }

        Result updateResult = product.Update(
            productName,
            productDescription,
            price,
            stockChange,
            reservedChange);

        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
