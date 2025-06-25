using FluentValidation;

namespace Server.Application.Products.UpdateProductReservedStock;

internal sealed class UpdateProductReservedStockCommandValidator : AbstractValidator<UpdateProductReservedStockCommand>
{
    public UpdateProductReservedStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");
    }
}
