namespace Server.Api.Controllers.Payments;

public record CreatePaymentRequest(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod);
 
