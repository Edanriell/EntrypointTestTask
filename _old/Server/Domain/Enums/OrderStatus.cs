namespace Domain.Enums;

public enum OrderStatus
{
	Created,
	PendingForPayment,
	Paid,
	InTransit,
	Delivered,
	Cancelled
}