using MediatR;
using Microsoft.Extensions.Logging;
using Product.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.Commands
{
    public sealed  class DeleteProductCommand : IRequest<bool>
    {
        public Guid Id { get; init; }
    }

    public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(
            IProductRepository repository,
            ILogger<DeleteProductCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", request.Id);

                var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID '{request.Id}' not found");
                }

                // Soft delete
                product.MarkAsDeleted();
                await _repository.UpdateAsync(product, cancellationToken);

                _logger.LogInformation("Product deleted with ID: {ProductId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", request.Id);
                throw;
            }
        }
    }
}
