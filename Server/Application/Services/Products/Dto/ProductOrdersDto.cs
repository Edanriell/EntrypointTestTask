using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Products.Dto;

public class ProductOrdersDto
{
    public int Id { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerSurname { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhoneNumber { get; set; }
    public string? CustomerAddress { get; set; }
    public short ProductOrderQuantity { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            {
                CreateMap<ProductOrderLink, ProductOrdersDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Order!.Id))
                    .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Order!.Status))
                    .ForMember(dest => dest.ProductOrderQuantity, opt => opt.MapFrom(src => src.Quantity))
                    .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Order!.User!.Name))
                    .ForMember(dest => dest.CustomerSurname, opt => opt.MapFrom(src => src.Order!.User!.Surname))
                    .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Order!.User!.Email))
                    .ForMember(dest => dest.CustomerPhoneNumber,
                        opt => opt.MapFrom(src => src.Order!.User!.PhoneNumber))
                    .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.Order!.User!.Address));
            }
        }
    }
}