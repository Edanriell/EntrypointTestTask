using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.GetOrders;

public sealed record GetOrdersQuery : IQuery<IReadOnlyList<OrdersResponse>>;
