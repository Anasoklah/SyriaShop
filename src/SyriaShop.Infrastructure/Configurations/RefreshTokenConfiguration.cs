using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(r => r.ReplacedByToken)
            .HasMaxLength(512);

        builder.HasIndex(r => r.Token)
            .IsUnique();

        builder.HasIndex(r => r.UserId);
    }
}
