namespace Domain.Entities;

public class ProductOrderLink
{
    [JsonIgnore] public Order Order { get; set; } = null!;
    public int OrderId { get; set; }
    public Product Product { get; set; } = null!;
    public int ProductId { get; set; }
    public short Quantity { get; set; }
}