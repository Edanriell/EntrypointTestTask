using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.UpdateProductPrice;

public sealed record UpdateProductPriceCommand(
    Guid ProductId,
    decimal NewPrice
) : ICommand;
 
 
