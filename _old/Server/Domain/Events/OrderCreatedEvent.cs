namespace Domain.Events;

public class OrderCreatedEvent(Order order) : BaseEvent
{
	public Order Order { get; } = order;
}