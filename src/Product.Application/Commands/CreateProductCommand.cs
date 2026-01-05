using MediatR;
using Microsoft.Extensions.Logging;
using Product.Domain.Interfaces.Factory;
using Product.Domain.Interfaces.Repositories;

namespace Product.Application.Commands
{
    public sealed class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int StockQuantity { get; init; }
        public string Category { get; init; } = string.Empty;
        public string Sku { get; init; } = string.Empty;
    }

    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _repository;
        private readonly IProductFactory _factory;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IProductRepository repository,
            IProductFactory factory,
            ILogger<CreateProductCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating product with SKU: {Sku}", request.Sku);

                // Check if SKU already exists
                if (await _repository.SkuExistsAsync(request.Sku, cancellationToken))
                {
                    throw new ApplicationException($"Product with SKU '{request.Sku}' already exists");
                }

                // Use factory to create product
                var product = _factory.CreateProduct(
                    request.Name,
                    request.Description,
                    request.Price,
                    request.StockQuantity,
                    request.Category,
                    request.Sku);

                // Add to repository
                await _repository.AddAsync(product, cancellationToken);

                _logger.LogInformation("Product created with ID: {ProductId}", product.Id);
                return product.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product with SKU: {Sku}", request.Sku);
                throw;
            }
        }
    }
}
