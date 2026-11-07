using SyriaShop.Domain;

namespace SyriaShop.Application.Identity;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    Task<ICollection<RefreshToken>> GetActiveTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    void Add(RefreshToken refreshToken);
}
