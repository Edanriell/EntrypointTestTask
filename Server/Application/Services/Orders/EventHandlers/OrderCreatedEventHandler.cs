using Domain.Events;

namespace Application.Services.Orders.EventHandlers;

public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    : INotificationHandler<OrderCreatedEvent>
{
    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("CREATED API Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}