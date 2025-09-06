using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.DiscountProduct;

public sealed record DiscountProductCommand(
    Guid ProductId,
    decimal NewPrice
) : ICommand;
 
 
