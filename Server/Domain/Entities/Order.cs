namespace Domain.Entities;

public class Order : BaseAuditableEntity
{
	[Key] [Required] public int         Id              { get; set; }
	public                  DateTime    CreatedAt       { get; set; }
	public                  DateTime    UpdatedAt       { get; set; }
	public                  OrderStatus Status          { get; set; }
	public                  string      ShippingName    { get; set; } = string.Empty;
	public                  Address     ShippingAddress { get; set; } = new();
	public                  Address     BillingAddress  { get; set; } = new();

	public string? AdditionalInformation { get; set; }

	// TODO Needs more testing here
	[JsonIgnore] public string UserId { get; set; } = string.Empty;
	[JsonIgnore] public User   User   { get; set; } = new();

	[JsonIgnore] public List<Product>? Products { get; set; }

	// This entity must be tested, do we need JsonIgnore here or upper
	// TODO Needs more testing here 
	[JsonIgnore] public List<ProductOrderLink>? OrderProducts { get; set; }

	public List<Payment>? Payments { get; set; }
	// Old
	// public ICollection<ProductOrderLink>? OrderProducts { get; set; }
}