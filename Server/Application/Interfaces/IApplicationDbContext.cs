using Domain.Entities;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    // We have Identity so this code is redundant
    // DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }

    DbSet<Product> Products { get; }

    // Old
    // If we set many-to-many relationship in old way we need this piece of code
    // DbSet<ProductOrderLink> ProductOrderLink { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}