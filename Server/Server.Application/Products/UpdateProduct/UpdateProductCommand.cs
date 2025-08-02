using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string? Name,
    string? Description,
    string? Currency,
    decimal? Price,
    int? StockChange
) : ICommand;
