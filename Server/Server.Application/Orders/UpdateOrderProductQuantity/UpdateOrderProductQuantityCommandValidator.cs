using FluentValidation;

namespace Server.Application.Orders.UpdateOrderProductQuantity;

public sealed class UpdateOrderProductQuantityCommandValidator : AbstractValidator<UpdateOrderProductQuantityCommand>
{
    public UpdateOrderProductQuantityCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.NewQuantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantity cannot exceed 1000 units");
    }
}
