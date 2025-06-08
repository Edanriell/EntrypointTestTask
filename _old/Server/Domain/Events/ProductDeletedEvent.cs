namespace Domain.Events;

public class ProductDeletedEvent(Product product) : BaseEvent
{
	public Product Product { get; } = product;
}