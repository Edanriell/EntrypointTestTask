using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.GetProducts;

public sealed record GetProductsQuery : IQuery<IReadOnlyList<ProductsResponse>>
{
}
