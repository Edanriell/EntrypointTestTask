namespace Domain.Events;

public class CategoryProductsUpdatedEvent(Category category) : BaseEvent
{
	public Category Category { get; } = category;
}