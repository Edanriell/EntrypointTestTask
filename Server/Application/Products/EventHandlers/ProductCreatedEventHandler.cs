using Domain.Events;

namespace Application.Products.EventHandlers;

public class ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
	: INotificationHandler<ProductCreatedEvent>
{
	public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("CREATED API Domain Event: {DomainEvent}", notification.GetType().Name);

		return Task.CompletedTask;
	}
}