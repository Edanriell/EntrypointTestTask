using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Orders.Commands.CreateOrder;

public class CreateOrderCommandResponseDto
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public string? ShipAddress { get; set; }
    public string? OrderInformation { get; set; }
    public List<ProductIdsWithQuantitiesDto>? OrderProducts { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Order, CreateOrderCommandResponseDto>()
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts));

            CreateMap<ProductOrderLink, ProductIdsWithQuantitiesDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        }
    }
}