using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Domain.Interfaces.Factory;
using Product.Domain.Interfaces.Repositories;
using Product.Infrastructure.Data;
using Product.Infrastructure.Factory;
using Product.Infrastructure.Repositories;

namespace Product.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            // Register Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // Register Factory
            services.AddScoped<IProductFactory, ProductFactory>();

            // Register Caching (if needed)
            services.AddDistributedMemoryCache();

            return services;
        }
    }
}
