namespace Domain.Events;

public class CategoryCreatedEvent(Category category) : BaseEvent
{
	public Category Category { get; } = category;
}