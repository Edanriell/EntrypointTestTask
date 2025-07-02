namespace Server.Api.Controllers.Orders;

public record UpdateOrderShippingAddressRequest(
    string Street,
    string City,
    string ZipCode,
    string Country
);
 
