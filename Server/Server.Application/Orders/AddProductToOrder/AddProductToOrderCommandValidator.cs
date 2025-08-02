using FluentValidation;

namespace Server.Application.Orders.AddProductToOrder;

internal sealed class AddProductToOrderCommandValidator : AbstractValidator<AddProductToOrderCommand>
{
    public AddProductToOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.Products)
            .NotEmpty()
            .WithMessage("At least one product must be specified");

        RuleForEach(x => x.Products)
            .SetValidator(new ProductItemValidator());
    }
}

internal sealed class ProductItemValidator : AbstractValidator<ProductItem>
{
    public ProductItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");
    }
}
