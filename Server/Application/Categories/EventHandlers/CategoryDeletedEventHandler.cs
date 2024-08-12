using Domain.Events;

namespace Application.Categories.EventHandlers;

public class CategoryDeletedEventHandler(ILogger<CategoryDeletedEventHandler> logger)
	: INotificationHandler<CategoryDeletedEvent>
{
	public Task Handle(CategoryDeletedEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("DELETED API Domain Event: {DomainEvent}", notification.GetType().Name);

		return Task.CompletedTask;
	}
}