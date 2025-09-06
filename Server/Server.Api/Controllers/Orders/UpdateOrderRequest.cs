namespace Server.Api.Controllers.Orders;

public record UpdateOrderRequest(
    string? Street,
    string? City,
    string? ZipCode,
    string? Country,
    string? Info
);
 
