using Application.Interfaces;
using Application.Services.Products.Dto;

namespace Application.Services.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<IResult>;

public class GetAllProductsQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
    : IRequestHandler<GetAllProductsQuery, IResult>
{
    public async Task<IResult> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        if (context.Products is null) return TypedResults.NotFound("No products has been found");

        var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

        if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, ProductDto[] result) dataTuple))
        {
            var query = context.Products
                .AsNoTracking()
                .Include(product => product.ProductOrders)!
                .ThenInclude(productOrderLink => productOrderLink.Order)
                .ThenInclude(order => order!.User);

            dataTuple.recordCount = await query.CountAsync(cancellationToken);

            if (dataTuple.recordCount == 0) throw new NotFoundException(request.ToString(), "Products");

            dataTuple.result =
                await query.ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

            memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
        }

        return TypedResults.Ok(
            new GetAllProductsQueryResponseDto<ProductDto[]>
            {
                Data = dataTuple.result,
                RecordCount = dataTuple.recordCount
            }
        );
    }
}