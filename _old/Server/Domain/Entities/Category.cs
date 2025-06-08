namespace Domain.Entities;

public class Category : BaseAuditableEntity
{
	public              int                        Id               { get; set; }
	public              string                     Name             { get; set; } = string.Empty;
	public              string                     Description      { get; set; } = string.Empty;
	public              List<Product>?             Products         { get; set; }
	[JsonIgnore] public List<ProductCategoryLink>? CategoryProducts { get; set; }
}