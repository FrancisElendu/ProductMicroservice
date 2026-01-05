using Microsoft.Extensions.Logging;
using Product.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductEntity = Product.Domain.Entities.Product;

namespace Product.Infrastructure.Factory
{
    public class ProductFactory : IProductFactory
    {
        private readonly ILogger<ProductFactory> _logger;

        public ProductFactory(ILogger<ProductFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ProductEntity CreateProduct(string name, string description, decimal price,
                                    int stockQuantity, string category, string sku)
        {
            try
            {
                _logger.LogInformation("Creating product with SKU: {Sku}", sku);

                var product = new ProductEntity(name, description, price, stockQuantity, category, sku);

                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product with SKU: {Sku}", sku);
                throw;
            }
        }

        public ProductEntity CreateProductFromDto(CreateProductDto dto)
        {
            return CreateProduct(
                dto.Name,
                dto.Description,
                dto.Price,
                dto.StockQuantity,
                dto.Category,
                dto.Sku);
        }
    }
}
