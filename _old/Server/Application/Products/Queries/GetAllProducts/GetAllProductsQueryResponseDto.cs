namespace Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryResponseDto<T>
{
	public T    Data        { get; set; } = default!;
	public int? RecordCount { get; set; }
}