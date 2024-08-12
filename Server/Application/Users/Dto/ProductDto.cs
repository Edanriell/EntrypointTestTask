using Domain.Entities;

namespace Application.Users.Dto;

public class ProductDto
{
	public int     Id        { get; set; }
	public string  Name      { get; set; } = null!;
	public decimal UnitPrice { get; set; }

	private class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Product, ProductDto>()
			   .ForMember(dest => dest.Id,        opt => opt.MapFrom(src => src.Id))
			   .ForMember(dest => dest.Name,      opt => opt.MapFrom(src => src.Name))
			   .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));
		}
	}
}