using ProductEntity = Product.Domain.Entities.Product;

namespace Product.Domain.Interfaces.Factory
{
    public interface IProductFactory
    {
        ProductEntity CreateProduct(string name, string description, decimal price,
                             int stockQuantity, string category, string sku);
        ProductEntity CreateProductFromDto(CreateProductDto dto);
    }
}
