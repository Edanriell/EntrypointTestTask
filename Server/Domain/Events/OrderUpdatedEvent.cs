namespace Domain.Events;

public class OrderUpdatedEvent(Order order) : BaseEvent
{
    public Order Order { get; } = order;
}