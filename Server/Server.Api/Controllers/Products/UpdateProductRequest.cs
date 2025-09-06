namespace Server.Api.Controllers.Products;

public record UpdateProductRequest(
    string? Name,
    string? Description,
    decimal? Price,
    string? Currency,
    int? Stock);
 
