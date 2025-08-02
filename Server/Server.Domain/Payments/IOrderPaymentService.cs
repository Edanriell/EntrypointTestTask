// using Server.Domain.Abstractions;
// using Server.Domain.Orders;
// using Server.Domain.Shared;
//
// namespace Server.Domain.Payments;
//
// public interface IPaymentService
// {
//     Result<Payment> ValidateAndCreatePayment(
//         Guid orderId,
//         Money amount,
//         PaymentMethod method,
//         Money remainingAmount,
//         IEnumerable<Payment> existingPayments);
//
//     bool CanAcceptPayment(OrderStatus status);
//     Result ValidatePaymentAmount(Money amount, Money remainingAmount, IEnumerable<Payment> pendingPayments);
// }



