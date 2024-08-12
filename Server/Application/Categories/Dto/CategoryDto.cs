using Domain.Entities;

namespace Application.Categories.Dto;

public class CategoryDto
{
	public int                       Id          { get; set; }
	public string                    Name        { get; set; } = string.Empty;
	public string                    Description { get; set; } = string.Empty;
	public List<CategoryProductsDto> Products    { get; set; }

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<Category, CategoryDto>()
			   .ForMember(dest => dest.Products,
					opt => opt.MapFrom(src => src.CategoryProducts));

			CreateMap<Category, CategoryDto>();
		}
	}
}