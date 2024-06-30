using Domain.Entities;

namespace Application.Services.Users.Dto;

public class OrderDto
{
    public int Quantity { get; set; }
    public ProductDto Product { get; set; } = null!;

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.OrderProducts.Sum(order => order.Quantity)))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.OrderProducts.FirstOrDefault()!.Product
                ));
        }
    }
}