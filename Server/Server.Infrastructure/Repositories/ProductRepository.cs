using Bookify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Products;

namespace Server.Infrastructure.Repositories;

internal sealed class ProductRepository
    : Repository<Product>,
        IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Product>()
            .Where(p => p.Status != ProductStatus.Deleted)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
 
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Product>()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Add(Product product) { DbContext.Add(product); }

    public void Update(Product product) { DbContext.Update(product); }

    public void Remove(Product product) { DbContext.Remove(product); }
}
