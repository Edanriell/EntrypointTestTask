namespace Domain.Events;

public class ProductCreatedEvent(Product product) : BaseEvent
{
    public Product Product { get; } = product;
}