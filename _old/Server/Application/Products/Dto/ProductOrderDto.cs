using Domain.Entities;
using Domain.Enums;

namespace Application.Products.Dto;

public class ProductOrderDto
{
	public int         OrderId             { get; set; }
	public OrderStatus OrderStatus         { get; set; }
	public string      CustomerName        { get; set; } = string.Empty;
	public string      CustomerSurname     { get; set; } = string.Empty;
	public string      CustomerEmail       { get; set; } = string.Empty;
	public string      CustomerPhoneNumber { get; set; } = string.Empty;
	public string      CustomerAddress     { get; set; } = string.Empty;
	public int         ProductId           { get; set; }
	public string      ProductName         { get; set; } = string.Empty;
	public int         ProductQuantity     { get; set; }

	private class MappingProfile : Profile
	{
		public MappingProfile()
		{
			{
				CreateMap<ProductOrderLink, ProductOrderDto>()
				   .ForMember(dest => dest.OrderId,         opt => opt.MapFrom(src => src.Order!.Id))
				   .ForMember(dest => dest.OrderStatus,     opt => opt.MapFrom(src => src.Order!.Status))
				   .ForMember(dest => dest.CustomerName,    opt => opt.MapFrom(src => src.Order!.User!.Name))
				   .ForMember(dest => dest.CustomerSurname, opt => opt.MapFrom(src => src.Order!.User!.Surname))
				   .ForMember(dest => dest.CustomerEmail,   opt => opt.MapFrom(src => src.Order!.User!.Email))
				   .ForMember(dest => dest.CustomerPhoneNumber,
						opt => opt.MapFrom(src => src.Order!.User!.PhoneNumber))
				   .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.Order!.User!.Address))
				   .ForMember(dest => dest.ProductId,       opt => opt.MapFrom(src => src.ProductId))
				   .ForMember(dest => dest.ProductName,     opt => opt.MapFrom(src => src.Product.Name))
				   .ForMember(dest => dest.ProductQuantity, opt => opt.MapFrom(src => src.Quantity));
			}
		}
	}
}