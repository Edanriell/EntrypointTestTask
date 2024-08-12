using Domain.Entities;

namespace Application.Products.Dto;

public class ProductDto
{
	public int                       Id                { get; set; }
	public string                    Code              { get; set; } = string.Empty;
	public string                    Name              { get; set; } = string.Empty;
	public string                    Description       { get; set; } = string.Empty;
	public decimal                   UnitPrice         { get; set; }
	public int                       UnitsInStock      { get; set; }
	public int                       UnitsOnOrder      { get; set; }
	public List<ProductOrderDto>?    ProductOrders     { get; set; }
	public List<ProductCategoryDto>? ProductCategories { get; set; }

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<Product, ProductDto>()
			   .ForMember(dest => dest.ProductOrders,
					opt => opt.MapFrom(src => src.ProductOrders));

			CreateMap<Product, ProductDto>()
			   .ForMember(dest => dest.ProductCategories, opt => opt.MapFrom(src => src.ProductCategories));
		}
	}
}