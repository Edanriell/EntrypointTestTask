using FluentValidation;

namespace Server.Application.Payments.RefundPayments;

public sealed class ProcessOrderRefundCommandValidator : AbstractValidator<RefundPaymentsCommand>
{
    public ProcessOrderRefundCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.RefundReason)
            .NotEmpty()
            .WithMessage("Refund reason is required")
            .MinimumLength(10)
            .WithMessage("Refund reason must be at least 10 characters long")
            .MaximumLength(1000)
            .WithMessage("Refund reason cannot exceed 1000 characters");
    }
}
