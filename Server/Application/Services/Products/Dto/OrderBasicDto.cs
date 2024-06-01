using Domain.Enums;

namespace Application.Services.Products.Dto;

public class OrderBasicDto
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public string? ShipAddress { get; init; }
    public string? OrderInformation { get; init; }
}