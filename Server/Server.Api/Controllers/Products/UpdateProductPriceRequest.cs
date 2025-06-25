namespace Server.Api.Controllers.Products;

public record UpdateProductPriceRequest(decimal Price, string Currency);
