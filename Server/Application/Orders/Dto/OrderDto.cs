using Domain.Entities;
using Domain.Enums;

namespace Application.Orders.Dto;

public class OrderDto
{
	public int                           Id                    { get; init; }
	public DateTime                      CreatedAt             { get; init; }
	public DateTime                      UpdatedAt             { get; init; }
	public OrderStatus                   Status                { get; init; }
	public string                        ShippingName          { get; init; } = string.Empty;
	public Address                       ShippingAddress       { get; init; } = null!;
	public Address                       BillingAddress        { get; init; } = null!;
	public string?                       AdditionalInformation { get; init; }
	public CustomerDto?                  Customer              { get; init; }
	public ICollection<ProductBasicDto>? Products              { get; init; }
	public ICollection<PaymentDto>?      Payments              { get; init; }

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<Order, OrderDto>()
			   .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.User))
			   .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderProducts!.Select(p =>
					new ProductBasicDto
					{
						Id          = p.ProductId,
						ProductName = p.Product.Name,
						UnitPrice   = p.Product.UnitPrice,
						Quantity    = p.Quantity
					})))
			   .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments!.Select(p => new PaymentDto
					{
						Id            = p.Id,
						Amount        = p.Amount,
						PaymentDate   = p.PaymentDate,
						PaymentMethod = p.PaymentMethod,
						TransactionId = p.TransactionId
					})));

			// TEST DO WE NEED THIS
			CreateMap<User, CustomerDto>();
			CreateMap<Product, ProductBasicDto>();
			CreateMap<Payment, PaymentDto>();
		}
	}
}