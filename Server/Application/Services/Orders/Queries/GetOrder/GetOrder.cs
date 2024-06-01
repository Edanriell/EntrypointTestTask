using Application.Interfaces;
using Application.Services.Orders.Dto;

namespace Application.Services.Orders.Queries.GetOrder;

public record GetOrderQuery : IRequest<IResult>
{
    public int Id { get; set; }
}

public class GetOrderQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
    : IRequestHandler<GetOrderQuery, IResult>
{
    public async Task<IResult> Handle(GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        if (context.Orders is null) return TypedResults.NotFound("No orders has been found");

        var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

        if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, OrderDto? result) dataTuple))
        {
            var entity = await context.Orders
                .AsNoTracking()
                .Include(order => order.User)
                .Include(order => order.OrderProducts)!
                .ThenInclude(productOrderLink => productOrderLink.Product)
                .Where(order => order.Id == request.Id)
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
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
            new GetOrderQueryResponseDto<OrderDto>
            {
                Data = dataTuple.result
            }
        );
    }
}