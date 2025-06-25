namespace Server.Api.Controllers.Products;

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int Stock,
    int? Reserved);
