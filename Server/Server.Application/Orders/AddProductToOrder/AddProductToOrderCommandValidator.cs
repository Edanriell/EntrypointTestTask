using FluentValidation;

namespace Server.Application.Orders.AddProductToOrder;
 
public sealed class AddProductToOrderCommandValidator : AbstractValidator<AddProductToOrderCommand>
{
    public AddProductToOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.Products)
            .NotNull()
            .WithMessage("Products cannot be null")
            .NotEmpty()
            .WithMessage("At least one product must be specified")
            .Must(HaveUniqueProducts)
            .WithMessage("Cannot add duplicate products in the same request");

        RuleForEach(x => x.Products)
            .SetValidator(new AddProductItemValidator());
    }

    private static bool HaveUniqueProducts(List<ProductItem> products)
    {
        if (products is null)
        {
            return true;
        }

        var productIds = products.Select(x => x.ProductId).ToList();
        return productIds.Count == productIds.Distinct().Count();
    }
}

public sealed class AddProductItemValidator : AbstractValidator<ProductItem>
{
    public AddProductItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantity cannot exceed 1000 per item");
    }
}
