using FluentValidation;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Products.CreateProduct;

internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(100)
            .WithMessage("Product name must not exceed 100 characters")
            .MustAsync(BeUniqueProductName)
            .WithMessage("Product with this name already exists");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Product description is required")
            .MaximumLength(1000)
            .WithMessage("Product description must not exceed 1000 characters");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Must(IsValidCurrency)
            .WithMessage("Invalid currency code. Supported currencies: USD, EUR.");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required.")
            .GreaterThan(0)
            .WithMessage("Product price must be greater than 0");

        RuleFor(x => x.TotalStock)
            .NotEmpty()
            .WithMessage("Stock is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Product stock cannot be negative");
    }

    private static bool IsValidCurrency(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            return false;
        }

        try
        {
            Currency.FromCode(currencyCode.ToUpper());
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> BeUniqueProductName(string name, CancellationToken cancellationToken)
    {
        Product? existingProduct = await _productRepository.GetByNameAsync(name, cancellationToken);
        return existingProduct is null;
    }
}
