namespace Application.Orders.Queries.GetOrder;

public class GetOrderQueryResponseDto<T>
{
	public T   Data        { get; set; } = default!;
	public int RecordCount { get; set; }
}