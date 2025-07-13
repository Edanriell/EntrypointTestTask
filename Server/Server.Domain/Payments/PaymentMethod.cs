using Server.Domain.Abstractions;

namespace Server.Domain.Payments;

public enum PaymentMethod
{
    CreditCard,
    DebitCard,
    PayPal,
    BankTransfer,
    Cash,
    Crypto,
    StoreCredit
}

public static class PaymentMethodExtensions
{
    public static Result<PaymentMethod> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PaymentMethod>(PaymentErrors.InvalidPaymentMethod);
        }

        return value.ToLowerInvariant() switch
        {
            "creditcard" or "credit_card" or "credit-card" => Result.Success(PaymentMethod.CreditCard),
            "debitcard" or "debit_card" or "debit-card" => Result.Success(PaymentMethod.DebitCard),
            "paypal" => Result.Success(PaymentMethod.PayPal),
            "banktransfer" or "bank_transfer" or "bank-transfer" => Result.Success(PaymentMethod.BankTransfer),
            "cash" => Result.Success(PaymentMethod.Cash),
            "crypto" or "cryptocurrency" => Result.Success(PaymentMethod.Crypto),
            "storecredit" or "store_credit" or "store-credit" => Result.Success(PaymentMethod.StoreCredit),
            _ => Result.Failure<PaymentMethod>(PaymentErrors.InvalidPaymentMethod)
        };
    }

    public static string ToDisplayString(this PaymentMethod paymentMethod)
    {
        return paymentMethod switch
        {
            PaymentMethod.CreditCard => "Credit Card",
            PaymentMethod.DebitCard => "Debit Card",
            PaymentMethod.PayPal => "PayPal",
            PaymentMethod.BankTransfer => "Bank Transfer",
            PaymentMethod.Cash => "Cash",
            PaymentMethod.Crypto => "Cryptocurrency",
            PaymentMethod.StoreCredit => "Store Credit",
            _ => paymentMethod.ToString()
        };
    }
}
