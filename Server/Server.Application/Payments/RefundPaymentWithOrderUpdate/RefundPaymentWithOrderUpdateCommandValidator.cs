using FluentValidation;

namespace Server.Application.Payments.RefundPaymentWithOrderUpdate;

public sealed class RefundPaymentWithOrderUpdateCommandValidator
    : AbstractValidator<RefundPaymentWithOrderUpdateCommand>
{
    public RefundPaymentWithOrderUpdateCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0)
            .WithMessage("Refund amount must be greater than 0")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Refund amount cannot exceed 999,999.99");

        RuleFor(x => x.RefundReason)
            .NotEmpty()
            .WithMessage("Refund reason is required")
            .MaximumLength(500)
            .WithMessage("Refund reason cannot exceed 500 characters");
    }
}
