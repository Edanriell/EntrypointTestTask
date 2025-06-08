namespace Domain.Entities;

public class ProductCategoryLink
{
	public int      CategoryId { get; set; }
	public Category Category   { get; set; } = new();
	public int      ProductId  { get; set; }
	public Product  Product    { get; set; } = new();
}