using FluentValidation;

namespace Server.Application.Orders.MarkOutForDelivery;

public sealed class MarkOutForDeliveryCommandValidator : AbstractValidator<MarkOutForDeliveryCommand>
{
    public MarkOutForDeliveryCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");
        
        RuleFor(x => x.EstimatedDeliveryDate)
            .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
            .WithMessage("Estimated delivery date must be in the future when provided")
            .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow.AddDays(7))
            .WithMessage("Estimated delivery date cannot be more than 7 days from now for out-for-delivery orders");
    }
}
