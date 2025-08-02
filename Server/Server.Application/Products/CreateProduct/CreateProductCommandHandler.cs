using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Products.CreateProduct;

internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        Result<ProductName> productNameResult = ProductName.Create(request.Name);
        if (productNameResult.IsFailure)
        {
            return Result.Failure<Guid>(productNameResult.Error);
        }

        Result<ProductDescription> descriptionResult = ProductDescription.Create(request.Description);
        if (descriptionResult.IsFailure)
        {
            return Result.Failure<Guid>(descriptionResult.Error);
        }

        Result<Currency> currencyResult = Currency.FromCode(request.Currency.ToUpper());
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Guid>(currencyResult.Error);
        }

        Result<Money> priceResult = Money.Create(request.Price, currencyResult.Value);
        if (priceResult.IsFailure)
        {
            return Result.Failure<Guid>(priceResult.Error);
        }

        Result<Quantity> stockResult = Quantity.CreateQuantity(request.TotalStock);
        if (stockResult.IsFailure)
        {
            return Result.Failure<Guid>(stockResult.Error);
        }

        Result<Quantity> reservedResult = Quantity.CreateQuantity(0);
        if (reservedResult.IsFailure)
        {
            return Result.Failure<Guid>(reservedResult.Error);
        }

        var product = Product.Create(
            productNameResult.Value,
            descriptionResult.Value,
            priceResult.Value,
            reservedResult.Value,
            stockResult.Value);

        _productRepository.Add(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id);
    }
}
