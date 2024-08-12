using Application.Interfaces;
using Application.Products.Dto;

namespace Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<IResult>;

public class GetAllProductsQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
	: IRequestHandler<GetAllProductsQuery, IResult>
{
	public async Task<IResult> Handle(GetAllProductsQuery request,
									  CancellationToken   cancellationToken)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, ProductDto[] result) dataTuple))
		{
			var query = context.Products
			   .AsNoTracking()
			   .AsSplitQuery()
			   .Include(product => product.ProductOrders)!
			   .ThenInclude(productOrderLink => productOrderLink.Order)
			   .ThenInclude(order => order!.User)
			   .Include(product => product.Categories);

			dataTuple.recordCount = await query.CountAsync(cancellationToken);

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = "No products found." });

			dataTuple.result =
				await query.ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
		}

		return TypedResults.Ok(
				new GetAllProductsQueryResponseDto<ProductDto[]>
				{
					Data        = dataTuple.result,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}