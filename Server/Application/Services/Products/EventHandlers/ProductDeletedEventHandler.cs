using Domain.Events;

namespace Application.Services.Products.EventHandlers;

public class ProductDeletedEventHandler(ILogger<ProductDeletedEventHandler> logger)
    : INotificationHandler<ProductDeletedEvent>
{
    public Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("DELETED API Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}