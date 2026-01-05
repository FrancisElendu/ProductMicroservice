using Microsoft.EntityFrameworkCore;
using Product.Domain.Interfaces.Repositories;
using Product.Infrastructure.Data;
using ProductEntity = Product.Domain.Entities.Product;

namespace Product.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<ProductEntity> _products;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _products = _context.Set<ProductEntity>();
        }

        public async Task<ProductEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _products
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductEntity> AddAsync(ProductEntity entity, CancellationToken cancellationToken = default)
        {
            var entry = await _products.AddAsync(entity, cancellationToken);
            return entry.Entity;
        }

        public async Task UpdateAsync(ProductEntity entity, CancellationToken cancellationToken = default)
        {
            _products.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(ProductEntity entity, CancellationToken cancellationToken = default)
        {
            entity.MarkAsDeleted();
            _products.Update(entity);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _products
                .AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<ProductEntity?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
        {
            return await _products
                .AsNoTracking()
                .Where(p => p.Category == category)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> GetProductsInStockAsync(CancellationToken cancellationToken = default)
        {
            return await _products
                .AsNoTracking()
                .Where(p => p.StockQuantity > 0)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _products
                .AnyAsync(p => p.Sku == sku, cancellationToken);
        }
    }
}
