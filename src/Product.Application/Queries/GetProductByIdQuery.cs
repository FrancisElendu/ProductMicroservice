using MediatR;
using Microsoft.Extensions.Logging;
using Product.Application.DTOs;
using Product.Domain.Interfaces.Repositories;
using ProductEntity = Product.Domain.Entities.Product;

namespace Product.Application.Queries
{
    public sealed class GetProductByIdQuery : IRequest<ProductDto>
    {
        public Guid Id { get; init; }
    }

    public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(
            IProductRepository repository,
            ILogger<GetProductByIdQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting product by ID: {ProductId}", request.Id);

                var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (product == null || product.IsDeleted)
                {
                    throw new KeyNotFoundException($"Product with ID '{request.Id}' not found");
                }

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID: {ProductId}", request.Id);
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
