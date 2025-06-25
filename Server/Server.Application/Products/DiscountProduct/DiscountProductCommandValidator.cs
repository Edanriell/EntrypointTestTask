using FluentValidation;

namespace Server.Application.Products.DiscountProduct;

internal sealed class DiscountProductCommandValidator : AbstractValidator<DiscountProductCommand>
{
    public DiscountProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.NewPrice)
            .GreaterThan(0)
            .WithMessage("New price must be greater than zero")
            .LessThan(1000000)
            .WithMessage("New price cannot exceed 1,000,000");
    }
}
