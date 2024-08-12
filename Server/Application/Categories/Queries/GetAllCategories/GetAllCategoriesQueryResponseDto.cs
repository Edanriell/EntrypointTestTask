namespace Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryResponseDto<T>
{
	public T    Data        { get; set; } = default!;
	public int? RecordCount { get; set; }
}