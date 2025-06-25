using FluentValidation;

namespace Server.Application.Payments.ProcessPaymentWithAutomaticOrderConfirmation;

public sealed class ProcessPaymentWithAutomaticOrderConfirmationCommandValidator
    : AbstractValidator<ProcessPaymentWithAutomaticOrderConfirmationCommand>
{
    public ProcessPaymentWithAutomaticOrderConfirmationCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.PaymentAmount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than 0")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Payment amount cannot exceed 999,999.99");
    }
}
