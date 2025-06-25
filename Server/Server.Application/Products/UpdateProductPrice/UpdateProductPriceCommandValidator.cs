using FluentValidation;

namespace Server.Application.Products.UpdateProductPrice;

internal sealed class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.NewPrice)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than 0")
            .LessThan(1000000)
            .WithMessage("Product price cannot exceed 1,000,000");
    }
}
