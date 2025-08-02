namespace Server.Api.Controllers.Products;

public record CreateProductRequest(
    string Name,
    string Description,
    string Currency,
    decimal Price,
    int Stock);
