namespace Application.Services.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryResponseDto<T>
{
    public T Data { get; set; } = default!;
    public int? RecordCount { get; set; }
}