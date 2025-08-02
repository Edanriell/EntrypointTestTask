using FluentValidation;

namespace Server.Application.Orders.ReturnOrder;

public sealed class ReturnOrderCommandValidator : AbstractValidator<ReturnOrderCommand>
{
    public ReturnOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.ReturnReason)
            .NotEmpty()
            .WithMessage("Return reason is required")
            .MaximumLength(500)
            .WithMessage("Return reason cannot exceed 500 characters");
    }
}
