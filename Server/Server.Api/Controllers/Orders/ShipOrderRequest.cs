using Server.Domain.Orders;

namespace Server.Api.Controllers.Orders;

public sealed record ShipOrderRequest(
    string TrackingNumber,
    Courier Courier,
    DateTime EstimatedDeliveryDate
);
