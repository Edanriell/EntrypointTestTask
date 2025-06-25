using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string? Name,
    string? Description,
    decimal? Price,
    int? StockChange,
    int? ReservedChange
) : ICommand;
