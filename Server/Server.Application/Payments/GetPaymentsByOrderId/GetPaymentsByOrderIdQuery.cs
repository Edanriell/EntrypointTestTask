using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.GetPaymentsByOrderId;

public sealed record GetPaymentsByOrderIdQuery(Guid OrderId) : IQuery<GetPaymentsByOrderIdResponse>;
 
