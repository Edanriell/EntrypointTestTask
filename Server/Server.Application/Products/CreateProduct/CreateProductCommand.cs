using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock
) : ICommand<Guid>;
