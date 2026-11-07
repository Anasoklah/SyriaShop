using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Slug)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.NameEn)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.NameAr)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.HasIndex(c => c.NameEn);

        builder.HasIndex(c => c.NameAr);

        builder.HasData(CatalogSeedData.Categories());
    }
}
