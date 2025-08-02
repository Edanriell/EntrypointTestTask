namespace Server.Domain.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class;

    void ClearChangeTracker();
}
