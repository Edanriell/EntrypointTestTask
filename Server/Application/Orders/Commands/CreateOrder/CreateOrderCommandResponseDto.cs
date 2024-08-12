using Application.Orders.Dto;
using Domain.Entities;
using Domain.Enums;

namespace Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandResponseDto
{
	public int                             Id       { get; init; }
	public OrderStatus                     Status   { get; init; }
	public List<ProductIdWithQuantityDto>? Products { get; init; }
	public List<PaymentDto>?               Payments { get; init; }

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<Order, CreateOrderCommandResponseDto>()
			   .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderProducts));

			CreateMap<ProductOrderLink, ProductIdWithQuantityDto>()
			   .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
			   .ForMember(dest => dest.Quantity,  opt => opt.MapFrom(src => src.Quantity));

			CreateMap<Payment, CreateOrderCommandResponseDto>()
			   .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => new PaymentDto
																		   {
																			   Id            = src.Id,
																			   Amount        = src.Amount,
																			   PaymentDate   = src.PaymentDate,
																			   PaymentMethod = src.PaymentMethod,
																			   TransactionId = src.TransactionId
																		   }));
		}
	}
}

//			CreateMap<Order, CreateOrderCommandResponseDto>()
//			   .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts));