using Application.Categories.Dto;
using Application.Interfaces;

namespace Application.Categories.Queries.GetPaginatedAndSortedCategories;

public record GetPaginatedAndSortedCategoriesQuery
	: IRequest<IResult>
{
	public int    PageIndex  { get; set; } = 0;
	public int    PageSize   { get; set; } = 10;
	public string SortColumn { get; set; } = "Id";
	public string SortOrder  { get; set; } = "DESC";
}

public class GetPaginatedAndSortedCategoriesQueryHandler(
	IApplicationDbContext context,
	IMapper               mapper,
	IMemoryCache          memoryCache)
	: IRequestHandler<GetPaginatedAndSortedCategoriesQuery,
		IResult>
{
	public async Task<IResult> Handle(
			GetPaginatedAndSortedCategoriesQuery request,
			CancellationToken                    cancellationToken
		)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, CategoryDto[] result) dataTuple))
		{
			var query = context.Categories
			   .AsNoTracking()
			   .AsSplitQuery()
			   .Include(category => category.CategoryProducts)
			   .OrderBy($"{request.SortColumn} {request.SortOrder}")
			   .Skip(request.PageIndex * request.PageSize)
			   .Take(request.PageSize);

			dataTuple.recordCount = await query.CountAsync(cancellationToken);

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = "No categories found matching the criteria." });

			dataTuple.result =
				await query.ProjectTo<CategoryDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
		}

		return TypedResults.Ok(
				new GetPaginatedAndSortedCategoriesQueryResponseDto<CategoryDto[]>
				{
					Data        = dataTuple.result,
					PageIndex   = request.PageIndex,
					PageSize    = request.PageSize,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}