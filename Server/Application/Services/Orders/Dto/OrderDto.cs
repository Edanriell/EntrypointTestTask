using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Orders.Dto;

public class OrderDto
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public string? ShipAddress { get; init; }
    public string? OrderInformation { get; init; }
    public CustomerDto? Customer { get; init; }
    public ICollection<ProductBasicDto>? Products { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderProducts.Select(p =>
                    new ProductBasicDto
                    {
                        Id = p.ProductId,
                        ProductName = p.Product.ProductName,
                        UnitPrice = p.Product.UnitPrice,
                        Quantity = p.Quantity
                    })));

            CreateMap<User, CustomerDto>();

            CreateMap<Product, ProductBasicDto>();
        }
    }
}