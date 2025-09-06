namespace Server.Api.Controllers.Orders;

public sealed record OutForDeliveryRequest(
    DateTime? EstimatedDeliveryDate
);
 
