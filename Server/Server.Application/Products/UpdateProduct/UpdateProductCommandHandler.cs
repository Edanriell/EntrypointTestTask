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

        // Handle price and currency updates
        Money? price = null;
        if (request.Price.HasValue || !string.IsNullOrWhiteSpace(request.Currency))
        {
            // Determine the currency to use
            Currency currency;
            if (!string.IsNullOrWhiteSpace(request.Currency))
            {
                // Currency is provided - use it
                try
                {
                    currency = Currency.FromCode(request.Currency.ToUpper());
                }
                catch (ApplicationException ex)
                {
                    return Result.Failure(new Error("Currency.Invalid", ex.Message));
                }
            }
            else
            {
                // Currency not provided - use existing currency
                currency = product.Price.Currency;
            }

            // Determine the price amount to use
            decimal priceAmount = request.Price ?? product.Price.Amount;

            // Create the new Money object
            Result<Money> priceResult = Money.Create(priceAmount, currency);
            if (priceResult.IsFailure)
            {
                return Result.Failure(priceResult.Error);
            }

            price = priceResult.Value;
        }

        Quantity? stockChange = null;
        if (request.StockChange.HasValue)
        {
            Result<Quantity> stockResult = Quantity.CreateStock(request.StockChange.Value);
            if (stockResult.IsFailure)
            {
                return Result.Failure(stockResult.Error);
            }

            stockChange = stockResult.Value;
        }

        Result updateProductResult = product.Update(
            productName, productDescription, price, stockChange);
        if (updateProductResult.IsFailure)
        {
            return Result.Failure(updateProductResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
