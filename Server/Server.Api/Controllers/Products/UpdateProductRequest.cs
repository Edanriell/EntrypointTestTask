namespace Server.Api.Controllers.Products;

public record UpdateProductRequest(
    string? Name,
    string? Description,
    decimal? Price,
    int? Stock,
    int? Reserved);
 
