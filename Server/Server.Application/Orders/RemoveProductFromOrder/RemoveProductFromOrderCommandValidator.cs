using FluentValidation;

namespace Server.Application.Orders.RemoveProductFromOrder;

public sealed class RemoveProductFromOrderCommandValidator : AbstractValidator<RemoveProductFromOrderCommand>
{
    public RemoveProductFromOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.ProductRemovals)
            .NotNull()
            .WithMessage("Product removals cannot be null")
            .NotEmpty()
            .WithMessage("At least one product removal must be specified")
            .Must(HaveUniqueProductIds)
            .WithMessage("Cannot specify duplicate product IDs");

        RuleForEach(x => x.ProductRemovals)
            .SetValidator(new ProductRemovalRequestValidator());
    }

    private static bool HaveUniqueProductIds(List<ProductRemovalRequest> removals)
    {
        if (removals == null)
        {
            return true;
        }

        var productIds = removals.Select(r => r.ProductId).ToList();
        return productIds.Count == productIds.Distinct().Count();
    }
}

public sealed class ProductRemovalRequestValidator : AbstractValidator<ProductRemovalRequest>
{
    public ProductRemovalRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID cannot be empty");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .When(x => x.Quantity.HasValue)
            .WithMessage("Quantity must be greater than zero when specified");
    }
}
