using Domain.Entities;

namespace Application.Categories.Dto;

public class CategoryProductsDto
{
	public int     Id           { get; set; }
	public string  Code         { get; set; } = string.Empty;
	public string  Name         { get; set; } = string.Empty;
	public string  Description  { get; set; } = string.Empty;
	public decimal UnitPrice    { get; set; }
	public int     UnitsInStock { get; set; }
	public int     UnitsOnOrder { get; set; }
	public string  Brand        { get; set; } = string.Empty;
	public double  Rating       { get; set; }
	public byte[]? Photo        { get; set; }

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<Product, CategoryProductsDto>();
		}
	}
}