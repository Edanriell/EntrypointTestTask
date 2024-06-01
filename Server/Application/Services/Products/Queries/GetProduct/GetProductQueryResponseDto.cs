namespace Application.Services.Products.Queries.GetProduct;

public class GetProductQueryResponseDto<T>
{
    public T Data { get; set; } = default!;
}