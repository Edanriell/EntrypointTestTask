namespace Application.Products.Queries.GetProduct;

public class GetProductQueryResponseDto<T>
{
	public T   Data        { get; set; } = default!;
	public int RecordCount { get; set; }
}