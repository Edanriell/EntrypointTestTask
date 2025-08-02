using FluentValidation;

namespace Server.Application.Orders.MarkOrderAsDelivered;

public sealed class MarkOrderAsDeliveredCommandValidator : AbstractValidator<MarkOrderAsDeliveredCommand>
{
    public MarkOrderAsDeliveredCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");
    }
}
 
