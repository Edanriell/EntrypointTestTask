using FluentValidation;

namespace Server.Application.Orders.RemoveProductFromOrder;

public sealed class RemoveProductFromOrderCommandValidator : AbstractValidator<RemoveProductFromOrderCommand>
{
    public RemoveProductFromOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.ProductIds)
            .NotNull()
            .WithMessage("Product IDs cannot be null")
            .NotEmpty()
            .WithMessage("At least one product ID must be specified")
            .Must(HaveUniqueProductIds)
            .WithMessage("Cannot specify duplicate product IDs");

        RuleForEach(x => x.ProductIds)
            .NotEmpty()
            .WithMessage("Product ID cannot be empty");
    }

    private static bool HaveUniqueProductIds(List<Guid> productIds)
    {
        if (productIds == null)
        {
            return true;
        }

        return productIds.Count == productIds.Distinct().Count();
    }
}
