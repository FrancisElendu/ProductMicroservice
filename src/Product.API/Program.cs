using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Product.API.Middleware;
using Product.Application;
using Product.Infrastructure;
using Product.Infrastructure.Data;
using System.Reflection;
using ProductEntity = Product.Domain.Entities.Product;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Microservice API",
        Version = "v1",
        Description = "Clean Architecture Product Microservice with CQRS and Factory Pattern"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Use API versioning
app.UseApiVersioning();

// Map health checks
app.MapHealthChecks("/health");

app.UseAuthorization();
app.MapControllers();

// Apply migrations and seed data
await ApplyMigrationsAsync(app);

app.Run();

static async Task ApplyMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.IsSqlServer())
        {
            await context.Database.MigrateAsync();
        }

        // Seed data if needed
        await SeedDataAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database");
        throw;
    }
}

static async Task SeedDataAsync(ApplicationDbContext context)
{
    if (!await context.Products.AnyAsync())
    {
        var products = new[]
        {
            new ProductEntity("Laptop", "High-performance laptop", 999.99m, 10, "Electronics", "LT-001"),
            new ProductEntity("Mouse", "Wireless mouse", 29.99m, 50, "Electronics", "MS-001"),
            new ProductEntity("Keyboard", "Mechanical keyboard", 89.99m, 30, "Electronics", "KB-001"),
            new ProductEntity("Monitor", "27-inch 4K monitor", 399.99m, 15, "Electronics", "MN-001")
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
