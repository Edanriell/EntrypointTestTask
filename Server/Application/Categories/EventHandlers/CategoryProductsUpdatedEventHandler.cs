using Domain.Events;

namespace Application.Categories.EventHandlers;

public class CategoryProductsUpdatedEventHandler(ILogger<CategoryProductsUpdatedEventHandler> logger)
	: INotificationHandler<CategoryProductsUpdatedEvent>
{
	public Task Handle(CategoryProductsUpdatedEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("UPDATED API Domain Event: {DomainEvent}", notification.GetType().Name);

		return Task.CompletedTask;
	}
}