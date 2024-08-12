using Application.Categories.Dto;
using Application.Interfaces;

namespace Application.Categories.Queries.GetCategory;

public record GetCategoryQuery : IRequest<IResult>
{
	public int Id { get; set; }
}

public class GetCategoryQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache)
	: IRequestHandler<GetCategoryQuery, IResult>
{
	public async Task<IResult> Handle(GetCategoryQuery  request,
									  CancellationToken cancellationToken)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, CategoryDto? result) dataTuple))
		{
			var entity = await context.Categories
							.AsNoTracking()
							.AsSplitQuery()
							.Include(category => category.CategoryProducts)
							.Where(category => category.Id == request.Id)
							.ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
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
				return TypedResults.NotFound(new { Message = $"Category with ID {request.Id} has not been found." });

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
		}

		return TypedResults.Ok(
				new GetCategoryQueryResponseDto<CategoryDto>
				{
					Data        = dataTuple.result!,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}