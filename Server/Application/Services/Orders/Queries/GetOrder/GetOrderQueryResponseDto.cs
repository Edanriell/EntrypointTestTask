namespace Application.Services.Orders.Queries.GetOrder;

public class GetOrderQueryResponseDto<T>
{
    public T Data { get; set; } = default!;
}