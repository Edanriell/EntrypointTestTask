namespace Domain.Events;

public class CategoryUpdatedEvent(Category category) : BaseEvent
{
	public Category Category { get; } = category;
}