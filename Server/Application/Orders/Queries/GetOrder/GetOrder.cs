using Application.Interfaces;
using Application.Orders.Dto;

namespace Application.Orders.Queries.GetOrder;

public record GetOrderQuery : IRequest<IResult>
{
	public int Id { get; set; }
}

public class GetOrderQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
	: IRequestHandler<GetOrderQuery, IResult>
{
	public async Task<IResult> Handle(GetOrderQuery     request,
									  CancellationToken cancellationToken)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, OrderDto? result) dataTuple))
		{
			var entity = await context.Orders
							.AsNoTracking()
							.AsSplitQuery()
							.Include(order => order.User)
							.Include(order => order.OrderProducts)!
							.ThenInclude(productOrderLink => productOrderLink.Product)
							.Include(order => order.Payments)
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
				dataTuple.result      = entity;
			}

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = $"Order with ID {request.Id} has not been found." });

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
		}

		return TypedResults.Ok(
				new GetOrderQueryResponseDto<OrderDto>
				{
					Data        = dataTuple.result!,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}