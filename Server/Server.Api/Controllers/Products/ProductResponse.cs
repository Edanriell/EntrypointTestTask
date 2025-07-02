namespace Server.Api.Controllers.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int Stock,
    int Reserved,
    string Status,
    DateTime CreatedAt,
    DateTime LastUpdatedAt);
 
