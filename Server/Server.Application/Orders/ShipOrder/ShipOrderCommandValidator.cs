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
            .WithMessage("Tracking number cannot exceed 100 characters")
            .Matches(@"^[A-Za-z0-9\-_]+$")
            .WithMessage("Tracking number can only contain letters, numbers, hyphens, and underscores");

        RuleFor(x => x.Courier)
            .IsInEnum()
            .WithMessage("Invalid courier selected");

        RuleFor(x => x.EstimatedDeliveryDate)
            .NotEmpty()
            .WithMessage("Estimated delivery date is required")
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("Estimated delivery date must be in the future")
            .Must(date => date <= DateTime.UtcNow.AddDays(30))
            .WithMessage("Estimated delivery date cannot be more than 30 days from now");
    }
}
