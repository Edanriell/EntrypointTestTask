using Domain.Entities;

namespace Application.Services.Products.Dto;

public class ProductDto
{
    public int Id { get; set; }
    public string? ProductName { get; set; }
    public short Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int UnitsInStock { get; set; }
    public int UnitsOnOrder { get; set; }
    public List<ProductOrdersDto>? ProductOrders { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductOrders,
                    opt => opt.MapFrom(src => src.ProductOrders));
        }
    }
}