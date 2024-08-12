using Domain.Events;

namespace Application.Categories.EventHandlers;

public class CategoryCreatedEventHandler(ILogger<CategoryCreatedEventHandler> logger)
	: INotificationHandler<CategoryCreatedEvent>
{
	public Task Handle(CategoryCreatedEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("CREATED API Domain Event: {DomainEvent}", notification.GetType().Name);

		return Task.CompletedTask;
	}
}