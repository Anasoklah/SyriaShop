using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Slug)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.NameEn)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.NameAr)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.DescriptionEn)
            .HasMaxLength(2000);

        builder.Property(p => p.DescriptionAr)
            .HasMaxLength(2000);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.HasIndex(p => p.CategoryId);

        builder.HasIndex(p => p.IsActive);

        builder.HasIndex(p => p.Price);

        builder.HasData(CatalogSeedData.Products());
    }
}
