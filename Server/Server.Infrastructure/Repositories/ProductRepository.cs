using Bookify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Abstractions;
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

    public async Task<Product?> GetByIdNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Product>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        // ✅ Create ProductName value object for comparison
        Result<ProductName> productName = ProductName.Create(name);

        return await DbContext
            .Set<Product>()
            .FirstOrDefaultAsync(p => p.Name == productName.Value, cancellationToken);
    }


    public void Add(Product product) { DbContext.Add(product); }

    public void Update(Product product) { DbContext.Update(product); }

    public void Remove(Product product) { DbContext.Remove(product); }
}
