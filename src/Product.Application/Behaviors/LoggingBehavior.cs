using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Product.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("Handling request: {RequestName} - {Request}", requestName, JsonSerializer.Serialize(request));

            var response = await next();

            _logger.LogInformation("Handled request: {RequestName}", requestName);
            return response;
        }
    }
}
