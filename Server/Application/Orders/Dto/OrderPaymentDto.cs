using Domain.Enums;

namespace Application.Orders.Dto;

public class OrderPaymentDto
{
	public decimal       Amount        { get; init; }
	public DateTime      PaymentDate   { get; init; }
	public PaymentMethod PaymentMethod { get; init; }
	public string        TransactionId { get; init; } = string.Empty;
}