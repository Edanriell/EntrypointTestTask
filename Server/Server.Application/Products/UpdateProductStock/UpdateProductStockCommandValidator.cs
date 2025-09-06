using FluentValidation;

namespace Server.Application.Products.UpdateProductStock;

internal sealed class UpdateProductStockCommandValidator : AbstractValidator<UpdateProductStockCommand>
{
    public UpdateProductStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.TotalStock)
            .NotEmpty()
            .WithMessage("Stock is required");
    }
}
 
