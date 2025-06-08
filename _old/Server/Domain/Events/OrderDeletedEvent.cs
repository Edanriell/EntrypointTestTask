namespace Domain.Events;

public class OrderDeletedEvent(Order order) : BaseEvent
{
	public Order Order { get; } = order;
}