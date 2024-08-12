namespace Application.Products.Queries.GetPaginatedSortedAndFilteredProducts;

public class GetPaginatedSortedAndFilteredProductsQueryResponseDto<T>
{
	public T    Data        { get; set; } = default!;
	public int? RecordCount { get; set; }
	public int? PageIndex   { get; set; }
	public int? PageSize    { get; set; }
}