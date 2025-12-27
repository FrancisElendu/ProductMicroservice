using ProductEntity = Product.Domain.Entities.Product;

namespace Product.Domain.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<ProductEntity>
    {
        Task<ProductEntity?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEntity>> GetProductsInStockAsync(CancellationToken cancellationToken = default);
        Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);
    }
}
