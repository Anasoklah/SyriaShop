using Microsoft.EntityFrameworkCore;
using SyriaShop.Application.Identity;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) =>
        context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

    public async Task<ICollection<RefreshToken>> GetActiveTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        return await context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > now)
            .ToListAsync(cancellationToken);
    }

    public void Add(RefreshToken refreshToken) => context.Add(refreshToken);
}
