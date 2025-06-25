namespace Server.Domain.Products;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(Product product);
    void Update(Product product);
    void Remove(Product product);
}
