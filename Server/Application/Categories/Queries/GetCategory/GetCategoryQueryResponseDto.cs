namespace Application.Categories.Queries.GetCategory;

public class GetCategoryQueryResponseDto<T>
{
	public T   Data        { get; set; } = default!;
	public int RecordCount { get; set; }
}