namespace Server.Api.Controllers.Products;

public record DiscountProductRequest(decimal NewPrice, string Currency);
