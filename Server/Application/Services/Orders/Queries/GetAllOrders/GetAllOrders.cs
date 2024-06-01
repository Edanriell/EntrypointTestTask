using Application.Interfaces;
using Application.Services.Orders.Dto;

namespace Application.Services.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery : IRequest<IResult>;

public class GetAllOrdersQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
    : IRequestHandler<GetAllOrdersQuery, IResult>
{
    public async Task<IResult> Handle(GetAllOrdersQuery request,
        CancellationToken cancellationToken)
    {
        if (context.Orders is null) return TypedResults.NotFound("No orders has been found");

        var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

        if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, OrderDto[] result) dataTuple))
        {
            var query = context.Orders
                .AsNoTracking()
                .Include(order => order.User)
                .Include(order => order.OrderProducts)!
                .ThenInclude(productOrderLink => productOrderLink.Product);

            dataTuple.recordCount = await query.CountAsync(cancellationToken);

            if (dataTuple.recordCount == 0) throw new NotFoundException(request.ToString(), "Orders");

            dataTuple.result =
                await query.ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

            memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
        }

        return TypedResults.Ok(
            new GetAllOrdersQueryResponseDto<OrderDto[]>
            {
                Data = dataTuple.result,
                RecordCount = dataTuple.recordCount
            }
        );
    }
}