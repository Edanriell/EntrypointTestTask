using Domain.Entities;

namespace Application.Products.Commands.CreateProduct;

public class CreateProductCommandResponseDto
{
	public int     Id           { get; set; }
	public string? ProductName  { get; set; }
	public decimal UnitPrice    { get; set; }
	public int     UnitsInStock { get; set; }

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<Product, CreateProductCommandResponseDto>();
		}
	}
}