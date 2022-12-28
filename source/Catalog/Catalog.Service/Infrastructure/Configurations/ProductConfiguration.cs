using Catalog.Service.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Service.Infrastructure.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public const string PRODUCT_TABLE_NAME = "products";
    public const string PRODUCT_PRIMARY_KEY_NAME = "pk_products";
    public const string PRODUCT_ID_COLUMN_NAME = "id";
    public const int PRODUCT_NAME_MAX_LENGTH = 64;
    public const string PRODUCT_NAME_COLUMN_NAME = "name";
    public const string PRODUCT_DESCRIPTION_COLUMN_NAME = "description";
    public const string PRODUCT_PRICE_COLUMN_NAME = "price";
    public const int PRODUCT_DESCRIPTION_MAX_LENGTH = 512;
    public const int PRODUCT_PRICE_PRECISION = 18;
    public const int PRODUCT_PRICE_SCALE = 4;
    public const string PRODUCT_INDEX_KEY_NAME = "ix_products";
    
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(PRODUCT_TABLE_NAME);

        builder.HasKey(product => product.Id)
            .HasName(PRODUCT_PRIMARY_KEY_NAME);

        builder.Property(product => product.Id)
            .HasColumnName(PRODUCT_ID_COLUMN_NAME);

        builder.Property(product => product.Name)
            .HasMaxLength(PRODUCT_NAME_MAX_LENGTH)
            .HasColumnName(PRODUCT_NAME_COLUMN_NAME);

        builder.Property(product => product.Description)
            .HasMaxLength(PRODUCT_DESCRIPTION_MAX_LENGTH)
            .HasColumnName(PRODUCT_DESCRIPTION_COLUMN_NAME);

        builder.Property(product => product.Price)
            .HasColumnName(PRODUCT_PRICE_COLUMN_NAME)
            .HasPrecision(PRODUCT_PRICE_PRECISION, PRODUCT_PRICE_SCALE);

        builder.HasIndex(product => new { product.Name, product.Description })
            .IsUnique()
            .HasDatabaseName(PRODUCT_INDEX_KEY_NAME);
    }
}