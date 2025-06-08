namespace Domain.Entities;

public class ProductOrderLink
{
	public              int     OrderId   { get; set; }
	[JsonIgnore] public Order   Order     { get; set; } = new();
	public              int     ProductId { get; set; }
	public              Product Product   { get; set; } = new();
	public              int     Quantity  { get; set; }
}