namespace Server.Domain.Payments;

public sealed record PaymentFailureReason
{
    public static readonly PaymentFailureReason InsufficientFunds = new("Insufficient funds");
    public static readonly PaymentFailureReason CardExpired = new("Card expired");
    public static readonly PaymentFailureReason TransactionDeclined = new("Transaction declined by bank");
    public static readonly PaymentFailureReason InvalidCardNumber = new("Invalid card number");
    public static readonly PaymentFailureReason PaymentProcessorError = new("Payment processor error");
    public static readonly PaymentFailureReason NetworkTimeout = new("Network timeout");
    public static readonly PaymentFailureReason Expired = new("Payment expired - more than 3 days since creation");
    public static readonly PaymentFailureReason Cancelled = new("Payment cancelled by customer");

    public static readonly IReadOnlyCollection<PaymentFailureReason> All = new[]
    {
        InsufficientFunds,
        CardExpired,
        TransactionDeclined,
        InvalidCardNumber,
        PaymentProcessorError,
        NetworkTimeout,
        Expired,
        Cancelled
    };

    private PaymentFailureReason(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static PaymentFailureReason Create(string failureReason)
    {
        return new PaymentFailureReason(failureReason);
    }

    public static PaymentFailureReason GetRandomReason()
    {
        PaymentFailureReason[] reasons = All.ToArray();
        return reasons[Random.Shared.Next(reasons.Length)];
    }
}
