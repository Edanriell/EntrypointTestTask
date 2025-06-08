using Application.Categories.Dto;
using Application.Interfaces;

namespace Application.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IResult>;

public class GetAllCategoriesQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
	: IRequestHandler<GetAllCategoriesQuery, IResult>
{
	public async Task<IResult> Handle(GetAllCategoriesQuery request,
									  CancellationToken     cancellationToken)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, CategoryDto[] result) dataTuple))
		{
			var query = context.Categories
			   .AsNoTracking()
			   .AsSplitQuery()
			   .Include(category => category.CategoryProducts);

			dataTuple.recordCount = await query.CountAsync(cancellationToken);

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = "No categories found." });

			dataTuple.result =
				await query.ProjectTo<CategoryDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
		}

		return TypedResults.Ok(
				new GetAllCategoriesQueryResponseDto<CategoryDto[]>
				{
					Data        = dataTuple.result,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}