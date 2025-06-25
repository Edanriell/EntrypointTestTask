using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.UpdateProductStock;

public sealed record UpdateProductStockCommand(
    Guid ProductId,
    int Stock
) : ICommand;
