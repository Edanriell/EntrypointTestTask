using Domain.Events;

namespace Application.Categories.EventHandlers;

public class CategoryUpdatedEventHandler(ILogger<CategoryUpdatedEventHandler> logger)
	: INotificationHandler<CategoryUpdatedEvent>
{
	public Task Handle(CategoryUpdatedEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("UPDATED API Domain Event: {DomainEvent}", notification.GetType().Name);

		return Task.CompletedTask;
	}
}