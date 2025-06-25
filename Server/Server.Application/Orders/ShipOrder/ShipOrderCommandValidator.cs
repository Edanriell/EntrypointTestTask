using FluentValidation;

namespace Server.Application.Orders.ShipOrder;

public sealed class ShipOrderCommandValidator : AbstractValidator<ShipOrderCommand>
{
    public ShipOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.TrackingNumber)
            .NotEmpty()
            .WithMessage("Tracking number is required")
            .MaximumLength(100)
            .WithMessage("Tracking number cannot exceed 100 characters");
    }
}
