using SyriaShop.Domain;

namespace SyriaShop.Application.Identity;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    Task<bool> TryRevokeActiveTokenAsync(string token, DateTimeOffset revokedAt, string? replacedByToken, CancellationToken cancellationToken = default);

    Task RevokeActiveTokensForUserAsync(Guid userId, DateTimeOffset revokedAt, CancellationToken cancellationToken = default);

    void Add(RefreshToken refreshToken);
}
