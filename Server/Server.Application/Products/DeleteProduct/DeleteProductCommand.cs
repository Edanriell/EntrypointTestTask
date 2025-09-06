using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.DeleteProduct;

public sealed record DeleteProductCommand(Guid ProductId) : ICommand;
 
 
