namespace Application.Services.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Code must be greater than 0");

        RuleFor(x => x.Code)
            .MinimumLength(4)
            .WithMessage("Code must be greater than 4");

        RuleFor(x => x.ProductName)
            .Length(6, 64)
            .WithMessage("ProductName must be at least 6 chars and no more than 64 chars long");

        RuleFor(x => x.Description)
            .Length(24, 128)
            .WithMessage("Product description must be at least 24 chars and no more than 128 chars long");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(1)
            .WithMessage("Unit price must be grater than 1");

        RuleFor(x => x.UnitsInStock)
            .GreaterThan((short)1)
            .WithMessage("Units in stock must be greater than 1");

        RuleFor(x => x.UnitsOnOrder)
            .GreaterThanOrEqualTo((short)0)
            .WithMessage("Units on order must contain a positive integer");
    }
}