using MediatR;
using Microsoft.Extensions.Logging;
using Product.Application.DTOs;
using Product.Domain.Interfaces.Repositories;
using ProductEntity = Product.Domain.Entities.Product;

namespace Product.Application.Queries
{
    public sealed class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
        {
            private readonly IProductRepository _repository;
            private readonly ILogger<GetAllProductsQueryHandler> _logger;

            public GetAllProductsQueryHandler(
                IProductRepository repository,
                ILogger<GetAllProductsQueryHandler> logger)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    _logger.LogInformation("Getting all products");

                    var products = await _repository.GetAllAsync(cancellationToken);
                    var activeProducts = products.Where(p => !p.IsDeleted);

                    return activeProducts.Select(MapToDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting all products");
                    throw;
                }
            }

            private static ProductDto MapToDto(ProductEntity product)
            {
                return new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Category = product.Category,
                    Sku = product.Sku,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    IsInStock = product.IsInStock()
                };
            }
        }
    }
}
