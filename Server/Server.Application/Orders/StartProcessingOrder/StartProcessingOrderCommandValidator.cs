using FluentValidation;

namespace Server.Application.Orders.StartProcessingOrder;

public sealed class StartProcessingOrderCommandValidator : AbstractValidator<StartProcessingOrderCommand>
{
    public StartProcessingOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");
    }
}
