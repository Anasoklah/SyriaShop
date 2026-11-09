using Microsoft.EntityFrameworkCore;
using SyriaShop.Application.Identity;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(AppDbContext context, TimeProvider timeProvider) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) =>
        context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

    public async Task<bool> TryRevokeActiveTokenAsync(
        string token,
        DateTimeOffset revokedAt,
        string? replacedByToken,
        CancellationToken cancellationToken = default)
    {
        var affected = await context.RefreshTokens
            .Where(r => r.Token == token && r.RevokedAt == null)
            .ExecuteUpdateAsync(
                updates => updates
                    .SetProperty(r => r.RevokedAt, revokedAt)
                    .SetProperty(r => r.ReplacedByToken, replacedByToken),
                cancellationToken);

        return affected > 0;
    }

    public async Task RevokeActiveTokensForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();

        await context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > now)
            .ExecuteUpdateAsync(updates => updates.SetProperty(r => r.RevokedAt, now), cancellationToken);
    }

    public void Add(RefreshToken refreshToken) => context.Add(refreshToken);
}
