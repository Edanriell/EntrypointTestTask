using Domain.Events;

namespace Application.Services.Products.EventHandlers;

public class ProductUpdatedEventHandler(ILogger<ProductUpdatedEventHandler> logger)
    : INotificationHandler<ProductUpdatedEvent>
{
    public Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("UPDATED API Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}