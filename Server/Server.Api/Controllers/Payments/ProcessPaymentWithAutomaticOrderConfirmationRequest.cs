namespace Server.Api.Controllers.Payments;

public record ProcessPaymentWithAutomaticOrderConfirmationRequest(
    Guid OrderId,
    decimal PaymentAmount
);
