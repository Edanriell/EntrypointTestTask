using FluentValidation;
using Server.Domain.Abstractions;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Application.Payments.AddPayment;

public sealed class AddPaymentCommandValidator : AbstractValidator<AddPaymentCommand>
{
    public AddPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Order ID cannot be empty");

        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage("Amount is required")
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Amount cannot exceed 1,000,000")
            .Must(HaveMaxTwoDecimalPlaces)
            .WithMessage("Amount cannot have more than 2 decimal places");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .NotNull()
            .WithMessage("Currency cannot be null")
            .Length(3)
            .WithMessage("Currency must be 3 characters long")
            .Must(BeValidCurrency)
            .WithMessage("Currency code is not supported. Supported currencies: USD, EUR");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required")
            .NotNull()
            .WithMessage("Payment method cannot be null")
            .Must(BeValidPaymentMethod)
            .WithMessage(
                "Payment method is not supported. Supported methods: CreditCard, DebitCard, PayPal, BankTransfer, Cash, Crypto, StoreCredit");
    }

    private static bool HaveMaxTwoDecimalPlaces(decimal amount)
    {
        decimal multiplied = amount * 100;
        return multiplied == Math.Floor(multiplied);
    }

    private static bool BeValidCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            return false;
        }

        Result<Currency> result = Currency.Create(currency);
        return result.IsSuccess;
    }

    private static bool BeValidPaymentMethod(string paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            return false;
        }

        Result<PaymentMethod> result = PaymentMethodExtensions.Create(paymentMethod);
        return result.IsSuccess;
    }
}
 
