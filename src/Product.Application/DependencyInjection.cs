using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Product.Application.Behaviors;
using System.Reflection;

namespace Product.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            });

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
