using Application.Interfaces;
using Application.Orders.Dto;

namespace Application.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery : IRequest<IResult>;

public class GetAllOrdersQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
	: IRequestHandler<GetAllOrdersQuery, IResult>
{
	public async Task<IResult> Handle(GetAllOrdersQuery request,
									  CancellationToken cancellationToken)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, OrderDto[] result) dataTuple))
		{
			var query = context.Orders
			   .AsNoTracking()
			   .AsSplitQuery()
			   .Include(order => order.User)
			   .Include(order => order.OrderProducts)!
			   .ThenInclude(productOrderLink => productOrderLink.Product)
			   .Include(order => order.Payments);

			dataTuple.recordCount = await query.CountAsync(cancellationToken);

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = "No orders found." });

			dataTuple.result =
				await query.ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
		}

		return TypedResults.Ok(
				new GetAllOrdersQueryResponseDto<OrderDto[]>
				{
					Data        = dataTuple.result,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}