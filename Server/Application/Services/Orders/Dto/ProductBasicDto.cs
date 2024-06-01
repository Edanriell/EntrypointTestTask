namespace Application.Services.Orders.Dto;

public class ProductBasicDto
{
    public int Id { get; set; }

    public string? ProductName { get; set; }

    public short Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}