using MediatR;
using Microsoft.Extensions.Logging;
using Product.Domain.Interfaces.Repositories;

namespace Product.Application.Commands
{
    public sealed class UpdateProductCommand : IRequest<bool>
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Category { get; init; } = string.Empty;
        public string Sku { get; init; } = string.Empty;
    }

    public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IProductRepository repository,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", request.Id);

                var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID '{request.Id}' not found");
                }

                // Check if SKU is being changed to an existing SKU
                if (product.Sku != request.Sku && await _repository.SkuExistsAsync(request.Sku, cancellationToken))
                {
                    throw new ApplicationException($"Product with SKU '{request.Sku}' already exists");
                }

                product.Update
                (
                    request.Name,
                    request.Description,
                    request.Price,
                    request.Category,
                    request.Sku
                );

                await _repository.UpdateAsync(product, cancellationToken);

                _logger.LogInformation("Product updated with ID: {ProductId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", request.Id);
                throw;
            }
        }
    }
}
