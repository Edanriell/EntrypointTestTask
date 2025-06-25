using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.UpdateProductReservedStock;

public sealed record UpdateProductReservedStockCommand(
    Guid ProductId,
    int ReservedStock
) : ICommand;
