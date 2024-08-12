namespace Application.Orders.Dto;

public class ProductBasicDto
{
	public int     Id          { get; init; }
	public string  ProductName { get; init; } = string.Empty;
	public int     Quantity    { get; init; }
	public decimal UnitPrice   { get; init; }
}