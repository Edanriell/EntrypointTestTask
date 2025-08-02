using FluentValidation;

namespace Server.Application.Orders.MarkReadyForShipment;

public sealed class MarkReadyForShipmentCommandValidator : AbstractValidator<MarkReadyForShipmentCommand>
{
    public MarkReadyForShipmentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");
    }
}
