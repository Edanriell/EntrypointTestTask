using Application.Interfaces;
using Application.Services.Products.Dto;

namespace Application.Services.Products.Queries.GetProduct;

public record GetProductQuery : IRequest<IResult>
{
    public int Id { get; set; }
}

public class GetProductQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
    : IRequestHandler<GetProductQuery, IResult>
{
    public async Task<IResult> Handle(GetProductQuery request,
        CancellationToken cancellationToken)
    {
        if (context.Products is null) return TypedResults.NotFound("No products has been found");

        var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

        if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, ProductDto? result) dataTuple))
        {
            var entity = await context.Products
                .AsNoTracking()
                .Include(product => product.ProductOrders)!
                .ThenInclude(productOrderLink => productOrderLink.Order)
                .ThenInclude(order => order!.User)
                .Where(product => product.Id == request.Id)
                .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                dataTuple.recordCount = 0;
            }
            else
            {
                dataTuple.recordCount = 1;
                dataTuple.result = entity;
            }

            memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
        }

        Guard.Against.NotFound(request.Id, dataTuple.result);

        return TypedResults.Ok(
            new GetProductQueryResponseDto<ProductDto>
            {
                Data = dataTuple.result
            }
        );
    }
}