using Domain.Events;

namespace Application.Orders.EventHandlers;

public class OrderUpdatedEventHandler(ILogger<OrderUpdatedEventHandler> logger)
	: INotificationHandler<OrderUpdatedEvent>
{
	public Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("UPDATED API Domain Event: {DomainEvent}", notification.GetType().Name);

		return Task.CompletedTask;
	}
}