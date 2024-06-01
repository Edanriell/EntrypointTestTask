namespace Application.Services.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id cannot be empty");

        RuleFor(x => x.ShipAddress)
            .Length(8, 128)
            .WithMessage("Ship address must be at least 8 chars and no more than 64 chars lon");

        RuleFor(x => x.OrderInformation)
            .Length(6, 128)
            .WithMessage("Order information must be at least 6 chars and no more than 36 chars long");

        RuleForEach(x => x.ProductIdsWithQuantities)
            .SetValidator(new ProductIdsWithQuantitiesDtoValidator());
    }
}

public class ProductIdsWithQuantitiesDtoValidator : AbstractValidator<ProductIdsWithQuantitiesDto>
{
    public ProductIdsWithQuantitiesDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product id must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan((short)0)
            .WithMessage("Quantity must be greater than 0");
    }
}