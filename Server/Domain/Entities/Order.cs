namespace Domain.Entities;

[Table("Orders")]
public class Order : BaseAuditableEntity
{
    [Required] public string UserId { get; set; }

    [Required] public DateTime CreatedAt { get; set; }

    [Required] public DateTime UpdatedAt { get; set; }

    [Required] public OrderStatus Status { get; set; } = OrderStatus.Created;

    [Required] [StringLength(80)] public string ShipAddress { get; set; } = string.Empty;

    [Required] [MaxLength(400)] public string OrderInformation { get; set; } = string.Empty;

    [JsonIgnore] public User? User { get; set; }

    [JsonIgnore] public List<Product> Products { get; set; } = new();

    public List<ProductOrderLink> OrderProducts { get; set; } = new();

    [Key] [Required] public int Id { get; set; }
    // Old
    // public ICollection<ProductOrderLink>? OrderProducts { get; set; }
}

// User field must be renamed to Customer =)