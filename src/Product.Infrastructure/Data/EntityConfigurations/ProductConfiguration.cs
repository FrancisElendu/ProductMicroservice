using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductEntity = Product.Domain.Entities.Product;
using System.Reflection;

namespace Product.Infrastructure.Data.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.StockQuantity)
                .IsRequired();

            builder.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.Sku)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasIndex(p => p.Category);
            builder.HasIndex(p => p.CreatedAt);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt);

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
