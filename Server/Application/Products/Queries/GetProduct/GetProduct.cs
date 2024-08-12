using Application.Interfaces;
using Application.Products.Dto;

namespace Application.Products.Queries.GetProduct;

public record GetProductQuery : IRequest<IResult>
{
	public int Id { get; set; }
}

public class GetProductQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
	: IRequestHandler<GetProductQuery, IResult>
{
	public async Task<IResult> Handle(GetProductQuery   request,
									  CancellationToken cancellationToken)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, ProductDto? result) dataTuple))
		{
			var entity = await context.Products
							.AsNoTracking()
							.AsSplitQuery()
							.Include(product => product.ProductOrders)!
							.ThenInclude(productOrderLink => productOrderLink.Order)
							.ThenInclude(order => order!.User)
							.Include(product => product.Categories)
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
				dataTuple.result      = entity;
			}

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = $"Product with ID {request.Id} has not been found." });

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
		}

		return TypedResults.Ok(
				new GetProductQueryResponseDto<ProductDto>
				{
					Data        = dataTuple.result!,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}