namespace Domain.Events;

public class CategoryDeletedEvent(Category category) : BaseEvent
{
	public Category Category { get; } = category;
}