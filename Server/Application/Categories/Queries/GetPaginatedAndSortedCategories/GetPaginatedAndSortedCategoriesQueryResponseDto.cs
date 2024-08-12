namespace Application.Categories.Queries.GetPaginatedAndSortedCategories;

public class GetPaginatedAndSortedCategoriesQueryResponseDto<T>
{
	public T    Data        { get; set; } = default!;
	public int? RecordCount { get; set; }
	public int? PageIndex   { get; set; }
	public int? PageSize    { get; set; }
}