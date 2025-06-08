using Domain.Events;

namespace Application.Orders.EventHandlers;

public class OrderDeletedEventHandler(ILogger<OrderDeletedEventHandler> logger)
	: INotificationHandler<OrderDeletedEvent>
{
	public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("DELETED API Domain Event: {DomainEvent}", notification.GetType().Name);

		return Task.CompletedTask;
	}
}