namespace Domain.Events;

public class ProductUpdatedEvent(Product product) : BaseEvent
{
	public Product Product { get; } = product;
}