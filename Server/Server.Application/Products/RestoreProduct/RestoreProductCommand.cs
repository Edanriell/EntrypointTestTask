using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.RestoreProduct;

public sealed record RestoreProductCommand(Guid ProductId) : ICommand;
