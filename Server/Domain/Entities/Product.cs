namespace Domain.Entities;

[Table("Products")]
public class Product : BaseAuditableEntity
{
    [Required] [MaxLength(80)] public string Code { get; set; } = null!;
    [Required] [MaxLength(100)] public string ProductName { get; set; } = null!;
    [Required] [MaxLength(300)] public string Description { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Required] public int UnitsInStock { get; set; }
    [Required] public int UnitsOnOrder { get; set; } = 0;
    public List<Order>? Orders { get; set; } = new();
    [JsonIgnore] public List<ProductOrderLink> ProductOrders { get; set; } = new();
    [Key] [Required] public int Id { get; set; }

    // Old
    // public ICollection<ProductOrderLink>? ProductOrders { get; set; }
}

// Missing Categories and Subcategories
// Add option to add multiple Photos
// Stock variants optional