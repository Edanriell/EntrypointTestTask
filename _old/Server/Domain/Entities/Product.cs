namespace Domain.Entities;

public class Product : BaseAuditableEntity
{
	public int     Id           { get; set; }
	public string  Code         { get; set; } = string.Empty;
	public string  Name         { get; set; } = string.Empty;
	public string  Description  { get; set; } = string.Empty;
	public decimal UnitPrice    { get; set; }
	public int     UnitsInStock { get; set; }
	public int     UnitsOnOrder { get; set; }
	public string  Brand        { get; set; } = string.Empty;

	public double Rating { get; set; }

	// Needs More testing
	public List<Order>? Orders { get; set; }

	[JsonIgnore] public List<ProductOrderLink>? ProductOrders { get; set; }

	// Needs More testing
	public              List<Category>?            Categories        { get; set; }
	[JsonIgnore] public List<ProductCategoryLink>? ProductCategories { get; set; }

	[Column(TypeName = "image")] public byte[]? Photo { get; set; }
	// Old
	// public ICollection<ProductOrderLink>? ProductOrders { get; set; }
}